using AsyncJobsTemplate.Infrastructure.Azure.StorageAccount;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.StorageAccount;

internal static class FilesData
{
    public static async Task<IReadOnlyList<StorageAccountFile>> GetInputFilesAsync(TestContextScope scope)
    {
        return await GetFilesAsync(scope, AzureStorageContainerType.Input);
    }

    public static async Task<IReadOnlyList<StorageAccountFile>> GetOutputFilesAsync(TestContextScope scope)
    {
        return await GetFilesAsync(scope, AzureStorageContainerType.Output);
    }

    public static async Task SaveFilesAsync(TestContextScope scope, List<JobFileInfo> files)
    {
        foreach (JobFileInfo file in files)
        {
            await SaveFileAsync(scope, AzureStorageContainerType.Output, file);
        }
    }

    private static async Task<IReadOnlyList<StorageAccountFile>> GetFilesAsync(
        TestContextScope scope,
        AzureStorageContainerType containerType
    )
    {
        BlobContainerClient containerClient = GetBlobContainerClient(scope, containerType);
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
        TestContextScope scope,
        AzureStorageContainerType containerType,
        JobFileInfo jobFileInfo
    )
    {
        BlobContainerClient containerClient = GetBlobContainerClient(scope, containerType);
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
        TestContextScope scope,
        AzureStorageContainerType containerType
    )
    {
        return containerType == AzureStorageContainerType.Input
            ? scope.StorageAccount.InputContainerClient
            : scope.StorageAccount.OutputContainerClient;
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
