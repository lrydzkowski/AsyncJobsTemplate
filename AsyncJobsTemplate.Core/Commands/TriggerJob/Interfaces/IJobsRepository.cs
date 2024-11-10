using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Core.Common.Models.Jobs;

namespace AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;

public interface IJobsRepository
{
    Task<Job> CreateJobAsync(JobToCreate job, CancellationToken cancellationToken);

    Task SaveErrorsAsync(Guid jobId, List<JobError> errors, CancellationToken cancellationToken);
}
