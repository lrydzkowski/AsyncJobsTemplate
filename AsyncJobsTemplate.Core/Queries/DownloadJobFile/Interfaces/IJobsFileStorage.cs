using AsyncJobsTemplate.Core.Common.Models.Jobs;

namespace AsyncJobsTemplate.Core.Queries.DownloadJobFile.Interfaces;

public interface IJobsFileStorage
{
    Task<JobFile?> GetOutputFileAsync(Guid jobId, CancellationToken cancellationToken);
}
