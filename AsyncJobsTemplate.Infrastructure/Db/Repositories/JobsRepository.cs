using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Core.Models;

namespace AsyncJobsTemplate.Infrastructure.Db.Repositories;

internal class JobsRepository : IJobsRepository
{
    public Task<Job> CreateJobAsync(JobToCreate job, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Job> SaveErrorsAsync(Guid jobId, List<JobError> errors, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
