using AsyncJobsTemplate.Core.Commands.RunJob.Interfaces;
using AsyncJobsTemplate.Core.Commands.RunJob.Models;
using AsyncJobsTemplate.Core.Extensions;
using AsyncJobsTemplate.Core.Jobs;
using AsyncJobsTemplate.Core.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        ProcessContext process = new();
        process = GetJobId(process, command);
        process = await GetJobAsync(process, cancellationToken);
        process = await SetRunningJobStatusAsync(process, cancellationToken);
        process = await RunJobAsync(process, cancellationToken);
        process = await SetFinishedJobStatusAsync(process, cancellationToken);

        RunJobResult result = new()
        {
            Result = process.JobExecutionResult
        };

        return result;
    }

    private ProcessContext GetJobId(ProcessContext process, RunJobCommand command)
    {
        string jobIdToParse = command.Request.JobId ?? "";
        bool parsingResult = Guid.TryParse(jobIdToParse, out Guid jobId);
        if (!parsingResult)
        {
            string errorCode = JobErrorCodes.ParseJobIdFailure;
            string errorMessage = "An unexpected error has occured in parsing a job GUID = '{JobGuid}'";
            process.HandleError(_logger, errorCode, errorMessage, jobIdToParse);

            return process;
        }

        process.JobId = jobId;

        return process;
    }

    private async Task<ProcessContext> GetJobAsync(ProcessContext process, CancellationToken cancellationToken)
    {
        if (process.HasErrors || process.JobId is null)
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

        IJob? job = _serviceProvider.GetKeyedService<IJob>(process.Job.JobCategoryName);
        if (job is null)
        {
            string errorCode = JobErrorCodes.GetJobTypeFailure;
            string errorMessage = "An unexpected error has occured in resolving a job type. Job type doesn't exist.";
            process.HandleError(_logger, errorCode, errorMessage);

            return process;
        }

        try
        {
            process.JobExecutionResult = await job.RunJobAsync(job, cancellationToken);
        }
        catch (Exception ex)
        {
            string errorCode = JobErrorCodes.GetJobTypeFailure;
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
                await _jobsRepository.SetJobStatusAsync(
                    (Guid)process.JobId,
                    JobStatus.Failed,
                    process.Errors,
                    cancellationToken
                );

                return process;
            }

            await _jobsRepository.SetJobStatusAsync((Guid)process.JobId, JobStatus.Finished, cancellationToken);
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
