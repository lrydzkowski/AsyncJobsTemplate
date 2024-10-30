using System.Net.Mime;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Core.Models;
using AsyncJobsTemplate.Infrastructure.Azure.Options;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using IJobsFileStorageTriggerJob = AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces.IJobsFileStorage;
using IJobsFileStorageRunJob = AsyncJobsTemplate.Core.Commands.RunJob.Interfaces.IJobsFileStorage;
using IJobsFileStorageDownloadJobFile = AsyncJobsTemplate.Core.Queries.DownloadJobFile.Interfaces.IJobsFileStorage;

namespace AsyncJobsTemplate.Infrastructure.Azure.StorageAccount;

internal class JobsFileStorage : IJobsFileStorageTriggerJob, IJobsFileStorageRunJob, IJobsFileStorageDownloadJobFile
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly AzureStorageAccountOptions _options;

    public JobsFileStorage(IOptions<AzureStorageAccountOptions> options, BlobServiceClient blobServiceClient)
    {
        _options = options.Value;
        _blobServiceClient = blobServiceClient;
    }

    public async Task<JobFile?> GetOutputFileAsync(Guid jobId, CancellationToken cancellationToken)
    {
        return await GetFileAsync(jobId, _options.InputContainerName, cancellationToken);
    }

    public async Task<JobFile?> GetInputFileAsync(Guid jobId, CancellationToken cancellationToken)
    {
        return await GetFileAsync(jobId, _options.InputContainerName, cancellationToken);
    }

    public async Task<SaveFileResult> SaveOutputFileAsync(Guid jobId, JobFile file, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(file.Content);

        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_options.OutputContainerName)!;
        string fileName = jobId.ToString();
        BlobClient blobClient = containerClient.GetBlobClient(fileName)!;
        await blobClient.UploadAsync(
            file.Content,
            new BlobUploadOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    [FileMetadata.FileOriginalName] = file.FileName,
                    [FileMetadata.FileContentName] = file.ContentType
                }
            },
            cancellationToken
        )!;

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
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_options.InputContainerName)!;
        string fileName = jobId.ToString();
        BlobClient blobClient = containerClient.GetBlobClient(fileName)!;
        await using Stream stream = file.OpenReadStream();
        await blobClient.UploadAsync(
            stream,
            new BlobUploadOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    [FileMetadata.FileOriginalName] = file.FileName,
                    [FileMetadata.FileContentName] = file.ContentType
                }
            },
            cancellationToken
        )!;

        SaveFileResult result = new()
        {
            FileReference = fileName
        };

        return result;
    }

    private async Task<JobFile?> GetFileAsync(Guid jobId, string containerName, CancellationToken cancellationToken)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName)!;
        string fileName = jobId.ToString();
        BlobClient blobClient = containerClient.GetBlobClient(fileName)!;
        if (!await blobClient.ExistsAsync(cancellationToken)!)
        {
            return null;
        }

        Stream content = await blobClient.OpenReadAsync(new BlobOpenReadOptions(false), cancellationToken)!;
        BlobProperties properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken)!;

        JobFile jobFile = new()
        {
            Content = content,
            ContentType = properties.Metadata?[FileMetadata.FileContentName] ?? MediaTypeNames.Application.Octet,
            FileName = properties.Metadata?[FileMetadata.FileOriginalName] ?? "unknown"
        };

        return jobFile;
    }
}
