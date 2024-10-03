using AsyncJobsTemplate.Core.Models;

namespace AsyncJobsTemplate.Core.Jobs;

internal interface IJobHandler
{
    Task<Job> RunJobAsync(Job job, CancellationToken cancellationToken);
}
