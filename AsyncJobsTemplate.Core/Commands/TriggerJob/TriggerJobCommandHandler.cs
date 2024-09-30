using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Core.Extensions;
using AsyncJobsTemplate.Core.Models;
using MediatR;
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

        ProcessContext process = new()
        {
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

            SaveFileResult saveFileResult = await _jobFileStorage.SaveFileAsync(
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
            await _jobsQueue.SendMessageAsync(process.JobId, cancellationToken);
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
