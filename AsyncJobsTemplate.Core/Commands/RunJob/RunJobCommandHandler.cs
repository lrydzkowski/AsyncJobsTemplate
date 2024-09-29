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

public class RunJobRequest
{
    public string? JobId { get; init; }
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
        Process process = new();
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

    private Process GetJobId(Process process, RunJobCommand command)
    {
        string jobIdToParse = command.Request.JobId ?? "";
        bool parsingResult = Guid.TryParse(jobIdToParse, out Guid jobId);
        if (!parsingResult)
        {
            process.HandleError(
                _logger,
                JobErrorCodes.ParseJobIdFailure,
                "An unexpected error has occured in parsing job guid = '{JobGuid}'",
                jobIdToParse
            );

            return process;
        }

        process.JobId = jobId;

        return process;
    }

    private async Task<Process> GetJobAsync(Process process, CancellationToken cancellationToken)
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
            process.HandleError(
                _logger,
                JobErrorCodes.GetJobFailure,
                ex,
                "An unexpected error has occured in getting a job."
            );

            return process;
        }

        if (job is null)
        {
            process.HandleError(
                _logger,
                JobErrorCodes.GetJobFailure,
                "An unexpected error has occured in getting a job. Job doesn't exist."
            );

            return process;
        }

        process.Job = job;

        return process;
    }

    private async Task<Process> SetRunningJobStatusAsync(
        Process process,
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
            process.HandleError(
                _logger,
                JobErrorCodes.SaveJobStatusFailure,
                ex,
                "An unexpected error has occurred in saving a job status = '{JobStatus}'.",
                JobStatus.Running
            );
        }

        return process;
    }

    private async Task<Process> RunJobAsync(Process process, CancellationToken cancellationToken)
    {
        if (process.HasErrors || process.Job is null)
        {
            return process;
        }

        IJob? job = _serviceProvider.GetKeyedService<IJob>(process.Job.JobCategoryName);
        if (job is null)
        {
            process.HandleError(
                _logger,
                JobErrorCodes.GetJobTypeFailure,
                "An unexpected error has occured in resolving a job type. Job type doesn't exist."
            );

            return process;
        }

        try
        {
            process.JobExecutionResult = await job.RunJobAsync(job, cancellationToken);
        }
        catch (Exception ex)
        {
            process.HandleError(
                _logger,
                JobErrorCodes.GetJobTypeFailure,
                ex,
                "An unexpected error has occured in running a job."
            );
        }

        return process;
    }

    private async Task<Process> SetFinishedJobStatusAsync(
        Process process,
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
            process.HandleError(
                _logger,
                JobErrorCodes.SaveJobStatusFailure,
                ex,
                "An unexpected error has occurred in saving a job status."
            );
        }

        return process;
    }
}
