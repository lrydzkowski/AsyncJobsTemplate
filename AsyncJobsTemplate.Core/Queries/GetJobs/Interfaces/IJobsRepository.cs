using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Shared.Models.Lists;

namespace AsyncJobsTemplate.Core.Queries.GetJobs.Interfaces;

public interface IJobsRepository
{
    public Task<PaginatedList<Job>> GetJobsAsync(ListParameters listParameters, CancellationToken cancellationToken);
}
