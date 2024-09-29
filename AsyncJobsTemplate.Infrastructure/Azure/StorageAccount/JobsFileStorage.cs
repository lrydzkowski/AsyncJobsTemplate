using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using Microsoft.AspNetCore.Http;

namespace AsyncJobsTemplate.Infrastructure.Azure.StorageAccount;

internal class JobsFileStorage : IJobsFileStorage
{
    public Task<SaveFileResult> SaveFileAsync(Guid jobId, IFormFile file, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
