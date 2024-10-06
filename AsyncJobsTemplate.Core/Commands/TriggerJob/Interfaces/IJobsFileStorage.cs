using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using Microsoft.AspNetCore.Http;

namespace AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;

public interface IJobsFileStorage
{
    Task<SaveFileResult> SaveInputFileAsync(Guid jobId, IFormFile file, CancellationToken cancellationToken);
}
