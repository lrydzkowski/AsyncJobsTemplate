using AsyncJobsTemplate.Core.Models;
using AsyncJobsTemplate.Core.Models.Lists;

namespace AsyncJobsTemplate.Core.Queries.GetJobs.Interfaces;

public interface IJobsRepository
{
    public Task<PaginatedList<Job>> GetJobsAsync(ListParameters listParameters, CancellationToken cancellationToken);
}
