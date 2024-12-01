using AsyncJobsTemplate.Infrastructure.Azure.Options;
using AsyncJobsTemplate.Infrastructure.Azure.StorageAccount;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.StorageAccount;

internal static class FilesData
{
    public static async Task<IReadOnlyList<StorageAccountFile>> GetInputFilesAsync(
        WebApplicationFactory<Program> webApplicationFactory
    )
    {
        return await GetFilesAsync(webApplicationFactory, AzureStorageContainerType.Input);
    }

    public static async Task<IReadOnlyList<StorageAccountFile>> GetOutputFilesAsync(
        WebApplicationFactory<Program> webApplicationFactory
    )
    {
        return await GetFilesAsync(webApplicationFactory, AzureStorageContainerType.Output);
    }

    public static async Task SaveFilesAsync(
        WebApplicationFactory<Program> webApplicationFactory,
        List<JobFileInfo> files
    )
    {
        foreach (JobFileInfo file in files)
        {
            await SaveFileAsync(webApplicationFactory, AzureStorageContainerType.Output, file);
        }
    }

    private static async Task<IReadOnlyList<StorageAccountFile>> GetFilesAsync(
        WebApplicationFactory<Program> webApplicationFactory,
        AzureStorageContainerType containerType
    )
    {
        BlobContainerClient containerClient = GetBlobContainerClient(webApplicationFactory, containerType);
        List<StorageAccountFile> files = [];
        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(BlobTraits.Metadata))
        {
            string content = await GetContentAsync(containerClient, blobItem);

            files.Add(
                new StorageAccountFile
                {
                    Name = blobItem.Name,
                    Content = content,
                    Metadata = blobItem.Metadata ?? new Dictionary<string, string>()
                }
            );
        }

        return files;
    }

    private static async Task SaveFileAsync(
        WebApplicationFactory<Program> webApplicationFactory,
        AzureStorageContainerType containerType,
        JobFileInfo jobFileInfo
    )
    {
        BlobContainerClient containerClient = GetBlobContainerClient(webApplicationFactory, containerType);
        string fileName = jobFileInfo.JobId.ToString();
        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(
            jobFileInfo.File.Content,
            new BlobUploadOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    [FileMetadata.FileOriginalName] = jobFileInfo.File.FileName,
                    [FileMetadata.FileContentName] = jobFileInfo.File.ContentType
                }
            },
            CancellationToken.None
        )!;
    }

    private static BlobContainerClient GetBlobContainerClient(
        WebApplicationFactory<Program> webApplicationFactory,
        AzureStorageContainerType containerType
    )
    {
        AzureStorageAccountOptions options = webApplicationFactory.Server.Services
            .GetRequiredService<IOptions<AzureStorageAccountOptions>>()
            .Value;
        BlobServiceClient serviceClient = webApplicationFactory.Server.Services.GetRequiredService<BlobServiceClient>();
        BlobContainerClient? containerClient = serviceClient.GetBlobContainerClient(
            containerType == AzureStorageContainerType.Input ? options.InputContainerName : options.OutputContainerName
        );

        return containerClient;
    }

    private static async Task<string> GetContentAsync(BlobContainerClient containerClient, BlobItem blobItem)
    {
        BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name!)!;
        BlobDownloadResult result = await blobClient.DownloadContentAsync()!;

        return result.Content?.ToString() ?? "";
    }

    private enum AzureStorageContainerType
    {
        Input,
        Output
    }
}
