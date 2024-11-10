using AsyncJobsTemplate.Core.Common.Models.Jobs;
using AsyncJobsTemplate.Core.Common.Models.Lists;

namespace AsyncJobsTemplate.Core.Queries.GetJobs.Interfaces;

public interface IJobsRepository
{
    public Task<PaginatedList<Job>> GetJobsAsync(ListParameters listParameters, CancellationToken cancellationToken);
}
