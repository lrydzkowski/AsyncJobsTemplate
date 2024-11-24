using AsyncJobsTemplate.Infrastructure.Azure.Options;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.StorageAccount;

internal class ContextScope : IAsyncDisposable
{
    private readonly BlobContainerClient _inputContainerClient;
    private readonly BlobContainerClient _outputContainerClient;

    public ContextScope(IServiceProvider serviceProvider)
    {
        AzureStorageAccountOptions options =
            serviceProvider.GetRequiredService<IOptions<AzureStorageAccountOptions>>().Value;
        BlobServiceClient serviceClient = serviceProvider.GetRequiredService<BlobServiceClient>();
        _inputContainerClient = serviceClient.GetBlobContainerClient(options.InputContainerName)!;
        _outputContainerClient = serviceClient.GetBlobContainerClient(options.OutputContainerName)!;
    }

    public async ValueTask DisposeAsync()
    {
        await RemoveAllFilesAsync(_inputContainerClient);
        await RemoveAllFilesAsync(_outputContainerClient);
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
