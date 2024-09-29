using AsyncJobsTemplate.Core.Models;

namespace AsyncJobsTemplate.Core.Queries.GetJob.Interfaces;

public interface IJobsRepository
{
    Task<Job?> GetJobAsync(Guid jobId, CancellationToken cancellationToken);
}
