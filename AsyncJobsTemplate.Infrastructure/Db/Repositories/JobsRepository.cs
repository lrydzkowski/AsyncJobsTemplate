using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Core.Models;
using IJobsRepositoryTriggerJob = AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces.IJobsRepository;
using IJobsRepositoryRunJob = AsyncJobsTemplate.Core.Commands.RunJob.Interfaces.IJobsRepository;
using IJobsRepositoryGetJob = AsyncJobsTemplate.Core.Queries.GetJob.Interfaces.IJobsRepository;

namespace AsyncJobsTemplate.Infrastructure.Db.Repositories;

internal class JobsRepository : IJobsRepositoryTriggerJob, IJobsRepositoryRunJob, IJobsRepositoryGetJob
{
    Task<Job?> IJobsRepositoryGetJob.GetJobAsync(Guid jobId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<Job?> IJobsRepositoryRunJob.GetJobAsync(Guid jobId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetJobStatusAsync(Guid jobId, JobStatus status, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetJobStatusAsync(
        Guid jobId,
        JobStatus status,
        List<JobError> errors,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public Task<Job> CreateJobAsync(JobToCreate job, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Job> SaveErrorsAsync(Guid jobId, List<JobError> errors, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
