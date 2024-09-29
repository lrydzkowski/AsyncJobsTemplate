using System.Text.Json.Nodes;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AsyncJobsTemplate.Core.Commands.TriggerJob;

public class TriggerJobCommand : IRequest<TriggerJobResult>
{
    public TriggerJobRequest Request { get; init; } = new();
}

public class TriggerJobResult
{
    public bool Result { get; init; }

    public Guid? JobId { get; init; }
}

public class TriggerJobRequest
{
    public string? JobCategoryName { get; init; }

    public IFormFile? File { get; init; }

    public JsonObject? Data { get; init; }
}

public class Operation
{
    public Guid JobId { get; set; }

    public string? InputFileReference { get; set; }

    public List<JobError> Errors { get; set; } = [];

    public bool HasErrors => Errors.Count > 0;
}

public class TriggerJobCommandHandler : IRequestHandler<TriggerJobCommand, TriggerJobResult>
{
    private readonly IJobsFileStorage _jobFileStorage;
    private readonly IJobsQueue _jobsQueue;
    private readonly IJobsRepository _jobsRepository;
    private readonly ILogger<TriggerJobCommand> _logger;

    public TriggerJobCommandHandler(
        IJobsFileStorage jobFileStorage,
        IJobsRepository jobsRepository,
        IJobsQueue jobsQueue,
        ILogger<TriggerJobCommand> logger
    )
    {
        _jobFileStorage = jobFileStorage;
        _jobsRepository = jobsRepository;
        _jobsQueue = jobsQueue;
        _logger = logger;
    }

    public async Task<TriggerJobResult> Handle(TriggerJobCommand command, CancellationToken cancellationToken)
    {
        TriggerJobRequest request = command.Request;

        Operation operation = new()
        {
            JobId = Guid.NewGuid()
        };

        operation = await SaveFileAsync(operation, request, cancellationToken);
        operation = await CreateJobAsync(operation, request, cancellationToken);
        operation = await SendMessageAsync(operation, cancellationToken);
        operation = await SaveErrorsAsync(operation, cancellationToken);

        TriggerJobResult result = new()
        {
            Result = !operation.HasErrors,
            JobId = operation.JobId
        };

        return result;
    }

    private async Task<Operation> SaveFileAsync(
        Operation operation,
        TriggerJobRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (request.File is null)
            {
                return operation;
            }

            SaveFileResult saveFileResult = await _jobFileStorage.SaveFileAsync(
                operation.JobId.ToString(),
                request.File,
                cancellationToken
            );
            operation.InputFileReference = saveFileResult.FileReference;

            return operation;
        }
        catch (Exception ex)
        {
            HandleError(operation, ex, "An unexpected error has occured in saving a file.");

            return operation;
        }
    }

    private async Task<Operation> CreateJobAsync(
        Operation operation,
        TriggerJobRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            JobToCreate jobToCreate = new()
            {
                JobId = operation.JobId,
                InputData = request.Data,
                InputFileReference = operation.InputFileReference,
                Errors = operation.Errors
            };
            await _jobsRepository.CreateJobAsync(jobToCreate, cancellationToken);
        }
        catch (Exception ex)
        {
            HandleError(operation, ex, "An unexpected error has occured in creating a job.");
        }

        return operation;
    }

    private async Task<Operation> SendMessageAsync(Operation operation, CancellationToken cancellationToken)
    {
        if (operation.HasErrors)
        {
            return operation;
        }

        try
        {
            await _jobsQueue.SendMessageAsync(operation.JobId, cancellationToken);
        }
        catch (Exception ex)
        {
            HandleError(operation, ex, "An unexpected error has occured in sending a message.");
        }

        return operation;
    }

    private async Task<Operation> SaveErrorsAsync(Operation operation, CancellationToken cancellationToken)
    {
        if (!operation.HasErrors)
        {
            return operation;
        }

        try
        {
            await _jobsRepository.SaveErrorsAsync(operation.JobId, operation.Errors, cancellationToken);
        }
        catch (Exception ex)
        {
            HandleError(operation, ex, "An unexpected error has occured in saving errors.");
        }

        return operation;
    }

    private void HandleError(Operation operation, Exception ex, string errorMsg)
    {
        _logger.LogError(ex, errorMsg);
        operation.Errors.Add(
            new JobError { Message = ex.Message, Exception = ex, ErrorCode = JobErrorCodes.SaveErrorsFailure }
        );
    }
}
