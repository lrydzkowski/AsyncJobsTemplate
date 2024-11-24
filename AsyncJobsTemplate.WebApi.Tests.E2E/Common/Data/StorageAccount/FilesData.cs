using AsyncJobsTemplate.Infrastructure.Azure.Options;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
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

    private static async Task<IReadOnlyList<StorageAccountFile>> GetFilesAsync(
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
        List<StorageAccountFile> files = [];
        if (containerClient is null)
        {
            return files;
        }

        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(BlobTraits.Metadata)!)
        {
            string content = await GetContentAsync(containerClient, blobItem);

            files.Add(
                new StorageAccountFile
                {
                    Name = blobItem.Name!,
                    Content = content,
                    Metadata = blobItem.Metadata ?? new Dictionary<string, string>()
                }
            );
        }

        return files;
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
