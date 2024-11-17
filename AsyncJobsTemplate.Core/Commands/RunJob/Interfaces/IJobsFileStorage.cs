using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Core.Common.Models;

namespace AsyncJobsTemplate.Core.Commands.RunJob.Interfaces;

public interface IJobsFileStorage
{
    Task<JobFile?> GetInputFileAsync(Guid jobId, CancellationToken cancellationToken);

    Task<SaveFileResult> SaveOutputFileAsync(Guid jobId, JobFile file, CancellationToken cancellationToken);
}
