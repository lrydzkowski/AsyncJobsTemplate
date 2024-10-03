using AsyncJobsTemplate.Core.Commands.RunJob.Interfaces;
using AsyncJobsTemplate.Core.Commands.RunJob.Models;
using AsyncJobsTemplate.Core.Extensions;
using AsyncJobsTemplate.Core.Jobs;
using AsyncJobsTemplate.Core.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    private readonly IJobsRepository _jobsRepository;
    private readonly ILogger<RunJobCommandHandler> _logger;
    private readonly IServiceProvider _serviceProvider;

    public RunJobCommandHandler(
        IJobsRepository jobsRepository,
        ILogger<RunJobCommandHandler> logger,
        IServiceProvider serviceProvider
    )
    {
        _jobsRepository = jobsRepository;
        _logger = logger;
        _serviceProvider = serviceProvider;
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
        if (process.HasErrors || process.JobId is null)
        {
            return process;
        }

        try
        {
            await _jobsRepository.SetJobStatusAsync((Guid)process.JobId, JobStatus.Running, cancellationToken);
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
            process.Job = await jobHandler.RunJobAsync(process.Job, cancellationToken);
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
            if (process.HasErrors)
            {
                List<JobError> errors = process.Job?.Errors ?? [];
                errors.AddRange(process.Errors);

                JobToUpdate jobToUpdateWithErrors = new()
                {
                    JobId = (Guid)process.JobId,
                    Status = JobStatus.Failed,
                    OutputData = process.Job?.OutputData,
                    OutputFileReference = process.Job?.OutputFileReference,
                    Errors = errors
                };

                await _jobsRepository.UpdateJobAsync(jobToUpdateWithErrors, cancellationToken);

                return process;
            }

            JobToUpdate jobToUpdate = new()
            {
                JobId = (Guid)process.JobId,
                Status = JobStatus.Finished,
                OutputData = process.Job?.OutputData,
                OutputFileReference = process.Job?.OutputFileReference
            };

            await _jobsRepository.UpdateJobAsync(jobToUpdate, cancellationToken);
        }
        catch (Exception ex)
        {
            string errorCode = JobErrorCodes.SaveJobStatusFailure;
            string errorMessage = "An unexpected error has occurred in saving a job status.";
            process.HandleError(_logger, errorCode, errorMessage, ex);
        }

        return process;
    }
}
