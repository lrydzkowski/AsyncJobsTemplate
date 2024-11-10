using AsyncJobsTemplate.Core.Common.Models.Jobs;

namespace AsyncJobsTemplate.Core.Queries.GetJob.Interfaces;

public interface IJobsRepository
{
    Task<Job?> GetJobAsync(Guid jobId, CancellationToken cancellationToken);
}
