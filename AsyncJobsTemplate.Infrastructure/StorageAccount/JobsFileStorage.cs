using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using Microsoft.AspNetCore.Http;

namespace AsyncJobsTemplate.Infrastructure.StorageAccount;

internal class JobsFileStorage : IJobsFileStorage
{
    public Task<SaveFileResult> SaveFileAsync(string fileName, IFormFile file, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
