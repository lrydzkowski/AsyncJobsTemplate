namespace AsyncJobsTemplate.Core.Jobs;

internal interface IJob
{
    Task<bool> RunJobAsync(IJob job, CancellationToken cancellationToken);
}
