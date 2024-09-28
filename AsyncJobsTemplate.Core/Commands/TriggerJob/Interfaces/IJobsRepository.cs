using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Core.Models;

namespace AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;

public interface IJobsRepository
{
    Task<Job> CreateJobAsync(JobToCreate job, CancellationToken cancellationToken);
}
