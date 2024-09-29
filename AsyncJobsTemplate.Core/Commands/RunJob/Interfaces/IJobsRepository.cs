using AsyncJobsTemplate.Core.Models;

namespace AsyncJobsTemplate.Core.Commands.RunJob.Interfaces;

public interface IJobsRepository
{
    Task<Job?> GetJobAsync(Guid jobId, CancellationToken cancellationToken);

    Task SetJobStatusAsync(Guid jobId, JobStatus status, CancellationToken cancellationToken);

    Task SetJobStatusAsync(Guid jobId, JobStatus status, List<JobError> errors, CancellationToken cancellationToken);
}
