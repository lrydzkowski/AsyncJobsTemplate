using AsyncJobsTemplate.Infrastructure.Azure.Options;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.StorageAccount;

internal class ContextScope : IAsyncDisposable
{
    public ContextScope(IServiceProvider serviceProvider)
    {
        AzureStorageAccountOptions options =
            serviceProvider.GetRequiredService<IOptions<AzureStorageAccountOptions>>().Value;
        BlobServiceClient serviceClient = serviceProvider.GetRequiredService<BlobServiceClient>();
        InputContainerClient = serviceClient.GetBlobContainerClient(options.InputContainerName)!;
        OutputContainerClient = serviceClient.GetBlobContainerClient(options.OutputContainerName)!;
    }

    public BlobContainerClient InputContainerClient { get; }

    public BlobContainerClient OutputContainerClient { get; }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await RemoveAllFilesAsync(InputContainerClient);
            await RemoveAllFilesAsync(OutputContainerClient);
        }
        catch
        {
            // ignored
        }
    }

    private async Task RemoveAllFilesAsync(BlobContainerClient containerClient)
    {
        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync()!)
        {
            BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name!)!;
            await blobClient.DeleteAsync()!;
        }
    }
}
