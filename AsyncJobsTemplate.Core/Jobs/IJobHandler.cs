using AsyncJobsTemplate.Core.Common.Models.Jobs;

namespace AsyncJobsTemplate.Core.Jobs;

internal interface IJobHandler
{
    string Description { get; }

    Task<JobExecutionOutput> RunJobAsync(JobExecutionInput input, CancellationToken cancellationToken);
}
