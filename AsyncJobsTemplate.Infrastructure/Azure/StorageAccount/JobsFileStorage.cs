using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Core.Models;
using AsyncJobsTemplate.Infrastructure.Azure.Options;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using IJobsFileStorageTriggerJob = AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces.IJobsFileStorage;
using IJobsFileStorageRunJob = AsyncJobsTemplate.Core.Commands.RunJob.Interfaces.IJobsFileStorage;

namespace AsyncJobsTemplate.Infrastructure.Azure.StorageAccount;

internal class JobsFileStorage : IJobsFileStorageTriggerJob, IJobsFileStorageRunJob
{
    private const string FileOriginalNameMetadataKey = "fileOriginalName";

    private readonly BlobServiceClient _blobServiceClient;
    private readonly AzureStorageAccountOptions _options;

    public JobsFileStorage(IOptions<AzureStorageAccountOptions> options, BlobServiceClient blobServiceClient)
    {
        _options = options.Value;
        _blobServiceClient = blobServiceClient;
    }

    public async Task<JobFile?> GetInputFileAsync(Guid jobId, CancellationToken cancellationToken)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_options.InputContainerName);
        string fileName = jobId.ToString();
        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        if (!await blobClient.ExistsAsync(cancellationToken))
        {
            return null;
        }

        using MemoryStream ms = new();
        await blobClient.DownloadToAsync(ms, cancellationToken);
        byte[] data = ms.ToArray();

        BlobProperties properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);

        JobFile jobFile = new()
        {
            Data = data,
            ContentType = properties.ContentType,
            FileName = properties.Metadata[FileOriginalNameMetadataKey]
        };

        return jobFile;
    }

    public async Task<SaveFileResult> SaveOutputFileAsync(Guid jobId, JobFile file, CancellationToken cancellationToken)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_options.OutputContainerName);
        string fileName = jobId.ToString();
        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        BinaryData binaryData = new(file.Data);
        await blobClient.UploadAsync(
            binaryData,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType },
                Metadata = new Dictionary<string, string> { [FileOriginalNameMetadataKey] = file.FileName }
            },
            cancellationToken
        );

        SaveFileResult result = new()
        {
            FileReference = fileName
        };

        return result;
    }

    public async Task<SaveFileResult> SaveInputFileAsync(
        Guid jobId,
        IFormFile file,
        CancellationToken cancellationToken
    )
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_options.InputContainerName);
        string fileName = jobId.ToString();
        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        await using Stream stream = file.OpenReadStream();
        await blobClient.UploadAsync(
            stream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType },
                Metadata = new Dictionary<string, string> { [FileOriginalNameMetadataKey] = file.FileName }
            },
            cancellationToken
        );

        SaveFileResult result = new()
        {
            FileReference = fileName
        };

        return result;
    }
}
