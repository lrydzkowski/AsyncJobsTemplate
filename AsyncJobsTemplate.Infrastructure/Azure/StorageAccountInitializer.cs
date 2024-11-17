using AsyncJobsTemplate.Infrastructure.Azure.Options;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.Infrastructure.Azure;

public static class StorageAccountInitializer
{
    public static void CreateStorageAccountContainers(this IServiceProvider serviceProvider)
    {
        AzureStorageAccountOptions options =
            serviceProvider.GetRequiredService<IOptions<AzureStorageAccountOptions>>().Value;
        BlobServiceClient blobServiceClient = serviceProvider.GetRequiredService<BlobServiceClient>();

        blobServiceClient.CreateBlobContainer(options.InputContainerName);
        blobServiceClient.CreateBlobContainer(options.OutputContainerName);
    }
}
