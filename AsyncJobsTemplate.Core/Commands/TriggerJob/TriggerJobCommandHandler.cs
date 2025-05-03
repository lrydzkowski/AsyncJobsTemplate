using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Core.Common.Extensions;
using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Shared.Extensions;
using AsyncJobsTemplate.Shared.Models.Context;
using AsyncJobsTemplate.Shared.Validators;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AsyncJobsTemplate.Core.Commands.TriggerJob;

public class TriggerJobCommand : IRequest<TriggerJobResult>, IRequestContextOperation
{
    public TriggerJobRequest Request { get; init; } = new();

    public required RequestContext RequestContext { get; init; }
}

public class TriggerJobRequest
{
    public string? JobCategoryName { get; init; }

    public IFormFile? File { get; init; }

    public object? Data { get; init; }
}

public class TriggerJobResult
{
    public bool Result { get; init; }

    public bool HasInternalErrors { get; init; }

    public Guid? JobId { get; init; }
}

public class TriggerJobCommandHandler : IRequestHandler<TriggerJobCommand, TriggerJobResult>
{
    private readonly IJobsFileStorage _jobFileStorage;
    private readonly IJobsQueueSender _jobsQueueSender;
    private readonly IJobsRepository _jobsRepository;
    private readonly ILogger<TriggerJobCommandHandler> _logger;
    private readonly RequestContextValidator _requestContextValidator;

    public TriggerJobCommandHandler(
        RequestContextValidator requestContextValidator,
        IJobsFileStorage jobFileStorage,
        IJobsRepository jobsRepository,
        IJobsQueueSender jobsQueueSender,
        ILogger<TriggerJobCommandHandler> logger
    )
    {
        _requestContextValidator = requestContextValidator;
        _jobFileStorage = jobFileStorage;
        _jobsRepository = jobsRepository;
        _jobsQueueSender = jobsQueueSender;
        _logger = logger;
    }

    public async Task<TriggerJobResult> Handle(TriggerJobCommand command, CancellationToken cancellationToken)
    {
        await _requestContextValidator.ValidateAndThrowIfInvalidAsync(command, cancellationToken);

        TriggerJobRequest request = command.Request;

        ProcessContext process = new()
        {
            UserEmail = command.RequestContext.User.UserEmail,
            JobId = Guid.NewGuid(),
            JobCategoryName = request.JobCategoryName!,
            InputFile = request.File,
            InputData = request.Data
        };
        process = await SaveInputFileAsync(process, cancellationToken);
        process = await CreateJobAsync(process, cancellationToken);
        process = await SendMessageAsync(process, cancellationToken);
        process = await SaveErrorsAsync(process, cancellationToken);

        TriggerJobResult result = new()
        {
            Result = !process.HasErrors,
            HasInternalErrors = process.HasInternalErrors,
            JobId = process.JobId
        };

        return result;
    }

    private async Task<ProcessContext> SaveInputFileAsync(ProcessContext process, CancellationToken cancellationToken)
    {
        try
        {
            if (process.InputFile is null)
            {
                return process;
            }

            SaveFileResult saveFileResult = await _jobFileStorage.SaveInputFileAsync(
                process.JobId,
                process.InputFile,
                cancellationToken
            );
            process.InputFileReference = saveFileResult.FileReference;

            return process;
        }
        catch (Exception ex)
        {
            string errorCode = JobErrorCodes.SaveFileFailure;
            string errorMessage = "An unexpected error has occured in saving a file.";
            process.HandleError(_logger, errorCode, errorMessage, ex);

            return process;
        }
    }

    private async Task<ProcessContext> CreateJobAsync(ProcessContext process, CancellationToken cancellationToken)
    {
        try
        {
            JobToCreate jobToCreate = new()
            {
                UserEmail = process.UserEmail,
                JobId = process.JobId,
                JobCategoryName = process.JobCategoryName,
                InputData = process.InputData,
                InputFileReference = process.InputFileReference
            };
            await _jobsRepository.CreateJobAsync(jobToCreate, cancellationToken);
        }
        catch (Exception ex)
        {
            string errorCode = JobErrorCodes.CreateJobFailure;
            string errorMessage = "An unexpected error has occured in creating a job.";
            process.HandleError(_logger, errorCode, errorMessage, ex);
        }

        return process;
    }

    private async Task<ProcessContext> SendMessageAsync(ProcessContext process, CancellationToken cancellationToken)
    {
        if (process.HasErrors)
        {
            return process;
        }

        try
        {
            await _jobsQueueSender.SendMessageAsync(process.JobId, cancellationToken);
        }
        catch (Exception ex)
        {
            string errorCode = JobErrorCodes.SendMessageFailure;
            string errorMessage = "An unexpected error has occured in sending a message.";
            process.HandleError(_logger, errorCode, errorMessage, ex);
        }

        return process;
    }

    private async Task<ProcessContext> SaveErrorsAsync(ProcessContext process, CancellationToken cancellationToken)
    {
        if (!process.HasErrors)
        {
            return process;
        }

        try
        {
            await _jobsRepository.SaveErrorsAsync(process.JobId, process.Errors, cancellationToken);
        }
        catch (Exception ex)
        {
            string errorCode = JobErrorCodes.SaveErrorsFailure;
            string errorMessage = "An unexpected error has occured in saving errors.";
            process.HandleError(_logger, errorCode, errorMessage, ex);
        }

        return process;
    }
}
