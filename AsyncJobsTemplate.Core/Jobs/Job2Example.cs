using AsyncJobsTemplate.Core.Models;

namespace AsyncJobsTemplate.Core.Jobs;

internal class Job2Example : IJob
{
    public const string Name = "job1";

    public Task<bool> RunJobAsync(IJob job, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
