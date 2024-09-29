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

        Process process = new()
        {
            JobId = Guid.NewGuid(),
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

    private async Task<Process> SaveInputFileAsync(Process process, CancellationToken cancellationToken)
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
            process.HandleError(
                _logger,
                JobErrorCodes.SaveFileFailure,
                ex,
                "An unexpected error has occured in saving a file."
            );

            return process;
        }
    }

    private async Task<Process> CreateJobAsync(Process process, CancellationToken cancellationToken)
    {
        try
        {
            JobToCreate jobToCreate = new()
            {
                JobId = process.JobId,
                InputData = process.InputData,
                InputFileReference = process.InputFileReference
            };
            await _jobsRepository.CreateJobAsync(jobToCreate, cancellationToken);
        }
        catch (Exception ex)
        {
            process.HandleError(
                _logger,
                JobErrorCodes.CreateJobFailure,
                ex,
                "An unexpected error has occured in creating a job."
            );
        }

        return process;
    }

    private async Task<Process> SendMessageAsync(Process process, CancellationToken cancellationToken)
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
            process.HandleError(
                _logger,
                JobErrorCodes.SendMessageFailure,
                ex,
                "An unexpected error has occured in sending a message."
            );
        }

        return process;
    }

    private async Task<Process> SaveErrorsAsync(Process process, CancellationToken cancellationToken)
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
            process.HandleError(
                _logger,
                JobErrorCodes.SaveErrorsFailure,
                ex,
                "An unexpected error has occured in saving errors."
            );
        }

        return process;
    }
}
