using AsyncJobsTemplate.Core.Commands.RunJob.Interfaces;
using AsyncJobsTemplate.Core.Commands.RunJob.Models;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Core.Common.Extensions;
using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Core.Jobs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProcessContext = AsyncJobsTemplate.Core.Commands.RunJob.Models.ProcessContext;

namespace AsyncJobsTemplate.Core.Commands.RunJob;

public class RunJobCommand : IRequest<RunJobResult>
{
    public RunJobRequest Request { get; init; } = new();
}

public class RunJobResult
{
    public bool Result { get; init; }
}

public class RunJobCommandHandler : IRequestHandler<RunJobCommand, RunJobResult>
{
    private readonly IJobsFileStorage _jobsFileStorage;
    private readonly IJobsRepository _jobsRepository;
    private readonly ILogger<RunJobCommandHandler> _logger;
    private readonly IServiceProvider _serviceProvider;

    public RunJobCommandHandler(
        IJobsRepository jobsRepository,
        ILogger<RunJobCommandHandler> logger,
        IServiceProvider serviceProvider,
        IJobsFileStorage jobsFileStorage
    )
    {
        _jobsRepository = jobsRepository;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _jobsFileStorage = jobsFileStorage;
    }

    public async Task<RunJobResult> Handle(RunJobCommand command, CancellationToken cancellationToken)
    {
        ProcessContext process = new()
        {
            JobId = command.Request.JobId
        };
        process = await GetJobAsync(process, cancellationToken);
        process = await SetRunningJobStatusAsync(process, cancellationToken);
        process = await RunJobAsync(process, cancellationToken);
        process = await SetFinishedJobStatusAsync(process, cancellationToken);

        RunJobResult result = new()
        {
            Result = !process.HasErrors
        };

        return result;
    }

    private async Task<ProcessContext> GetJobAsync(ProcessContext process, CancellationToken cancellationToken)
    {
        if (process.JobId is null)
        {
            return process;
        }

        Job? job;

        try
        {
            job = await _jobsRepository.GetJobAsync((Guid)process.JobId, cancellationToken);
        }
        catch (Exception ex)
        {
            string errorCode = JobErrorCodes.GetJobFailure;
            string errorMessage = "An unexpected error has occured in getting a job.";
            process.HandleError(_logger, errorCode, errorMessage, ex);

            return process;
        }

        if (job is null)
        {
            string errorCode = JobErrorCodes.GetJobFailure;
            string errorMessage = "An unexpected error has occured in getting a job. Job doesn't exist.";
            process.HandleError(_logger, errorCode, errorMessage);

            return process;
        }

        process.Job = job;

        return process;
    }

    private async Task<ProcessContext> SetRunningJobStatusAsync(
        ProcessContext process,
        CancellationToken cancellationToken
    )
    {
        if (process.HasErrors || process.Job is null)
        {
            return process;
        }

        try
        {
            await _jobsRepository.SetJobStatusAsync(process.Job.JobId, JobStatus.Running, cancellationToken);
        }
        catch (Exception ex)
        {
            string errorCode = JobErrorCodes.SaveJobStatusFailure;
            string errorMessage = "An unexpected error has occurred in saving a job status = '{JobStatus}'.";
            process.HandleError(_logger, errorCode, errorMessage, ex, JobStatus.Running);
        }

        return process;
    }

    private async Task<ProcessContext> RunJobAsync(ProcessContext process, CancellationToken cancellationToken)
    {
        if (process.HasErrors || process.Job is null)
        {
            return process;
        }

        IJobHandler? jobHandler = _serviceProvider.GetKeyedService<IJobHandler>(process.Job.JobCategoryName);
        if (jobHandler is null)
        {
            string errorCode = JobErrorCodes.GetJobTypeFailure;
            string errorMessage = "An unexpected error has occured in resolving a job type. Job type doesn't exist.";
            process.HandleError(_logger, errorCode, errorMessage);

            return process;
        }

        try
        {
            JobExecutionInput input = new()
            {
                JobId = process.Job.JobId,
                InputData = process.Job.InputData,
                InputFile = process.Job.InputFileReference is null
                    ? null
                    : await _jobsFileStorage.GetInputFileAsync(process.Job.JobId, cancellationToken)
            };

            JobExecutionOutput output = await jobHandler.RunJobAsync(input, cancellationToken);

            process.Job.OutputData = output.OutputData;
            if (output.OutputFile is not null)
            {
                SaveFileResult result = await _jobsFileStorage.SaveOutputFileAsync(
                    process.Job.JobId,
                    output.OutputFile,
                    cancellationToken
                );
                process.Job.OutputFileReference = result.FileReference;
            }
        }
        catch (Exception ex)
        {
            string errorCode = JobErrorCodes.RunJobFailure;
            string errorMessage = "An unexpected error has occured in running a job.";
            process.HandleError(_logger, errorCode, errorMessage, ex);
        }

        return process;
    }

    private async Task<ProcessContext> SetFinishedJobStatusAsync(
        ProcessContext process,
        CancellationToken cancellationToken
    )
    {
        if (process.JobId is null)
        {
            return process;
        }

        try
        {
            JobStatus status = process.HasErrors ? JobStatus.Failed : JobStatus.Finished;
            await _jobsRepository.UpdateJobAsync(BuildJobToUpdate(process, status), cancellationToken);
        }
        catch (Exception ex)
        {
            string errorCode = JobErrorCodes.SaveJobStatusFailure;
            string errorMessage = "An unexpected error has occurred in saving a job status.";
            process.HandleError(_logger, errorCode, errorMessage, ex);
        }

        return process;
    }

    private static JobToUpdate BuildJobToUpdate(ProcessContext process, JobStatus status)
    {
        List<JobError> errors = process.Job?.Errors ?? [];
        errors.AddRange(process.Errors);

        JobToUpdate jobToUpdate = new()
        {
            JobId = process.Job!.JobId,
            Status = status,
            OutputData = process.Job?.OutputData,
            OutputFileReference = process.Job?.OutputFileReference,
            Errors = errors.Count > 0 ? errors : null
        };

        return jobToUpdate;
    }
}
