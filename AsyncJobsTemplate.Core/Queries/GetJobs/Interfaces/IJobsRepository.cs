using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Shared.Models.Lists;

namespace AsyncJobsTemplate.Core.Queries.GetJobs.Interfaces;

public interface IJobsRepository
{
    public Task<PaginatedList<Job>> GetJobsAsync(
        string userEmail,
        ListParameters listParameters,
        CancellationToken cancellationToken
    );
}
