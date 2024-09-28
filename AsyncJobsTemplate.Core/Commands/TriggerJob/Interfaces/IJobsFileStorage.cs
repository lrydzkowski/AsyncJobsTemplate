using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using Microsoft.AspNetCore.Http;

namespace AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;

public interface IJobsFileStorage
{
    Task<SaveFileResult> SaveFileAsync(string fileName, IFormFile file, CancellationToken cancellationToken);
}
