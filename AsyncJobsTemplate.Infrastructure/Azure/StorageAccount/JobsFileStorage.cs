using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Infrastructure.Azure.Options;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.Infrastructure.Azure.StorageAccount;

internal class JobsFileStorage : IJobsFileStorage
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly AzureStorageAccountOptions _options;

    public JobsFileStorage(IOptions<AzureStorageAccountOptions> options, BlobServiceClient blobServiceClient)
    {
        _options = options.Value;
        _blobServiceClient = blobServiceClient;
    }

    public async Task<SaveFileResult> SaveFileAsync(Guid jobId, IFormFile file, CancellationToken cancellationToken)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_options.ContainerName);
        string fileName = jobId.ToString();
        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        await using Stream stream = file.OpenReadStream();
        await blobClient.UploadAsync(
            stream,
            new BlobHttpHeaders { ContentType = file.ContentType },
            cancellationToken: cancellationToken
        );

        SaveFileResult result = new()
        {
            FileReference = fileName
        };

        return result;
    }
}
