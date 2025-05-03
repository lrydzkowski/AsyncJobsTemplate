using AsyncJobsTemplate.Core;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Infrastructure.Azure.Authentication;
using AsyncJobsTemplate.Infrastructure.Azure.Options;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common;
using AsyncJobsTemplate.Infrastructure.Azure.StorageAccount;
using Azure.Core;
using Azure.Storage.Blobs;
using HealthChecks.Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using IJobsFileStorageRunJob = AsyncJobsTemplate.Core.Commands.RunJob.Interfaces.IJobsFileStorage;
using IJobsFileStorageTriggerJob = AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces.IJobsFileStorage;
using IJobsFileStorageDownloadJobFile = AsyncJobsTemplate.Core.Queries.DownloadJobFile.Interfaces.IJobsFileStorage;

namespace AsyncJobsTemplate.Infrastructure.Azure;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddServices()
            .AddOptions(configuration)
            .AddAzureBlobServiceClient()
            .AddHealthCheck()
            .AddJobsQueue()
            .AddServiceBusCommonServices();
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddScoped<IJobsFileStorageRunJob, JobsFileStorage>()
            .AddScoped<IJobsFileStorageTriggerJob, JobsFileStorage>()
            .AddScoped<IJobsFileStorageDownloadJobFile, JobsFileStorage>()
            .AddSingleton<IAccessTokenProvider, AccessTokenProvider>();
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddOptionsType<AzureStorageAccountOptions>(configuration, AzureStorageAccountOptions.Position)
            .AddOptionsType<AzureAdOptions>(configuration, AzureAdOptions.Position)
            .AddOptionsType<AzureServiceBusOptions>(configuration, AzureServiceBusOptions.Position);
    }

    private static IServiceCollection AddAzureBlobServiceClient(this IServiceCollection services)
    {
        services.AddAzureClients(azureClientFactoryBuilder =>
            {
                azureClientFactoryBuilder.AddClient<BlobServiceClient, BlobClientOptions>((_, serviceProvider) =>
                    {
                        AzureStorageAccountOptions options = serviceProvider
                            .GetRequiredService<IOptions<AzureStorageAccountOptions>>()
                            .Value;
                        if (!string.IsNullOrWhiteSpace(options.ConnectionString))
                        {
                            return new BlobServiceClient(options.ConnectionString);
                        }

                        Uri blobUri = new($"https://{options.Name}.blob.core.windows.net");

                        IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
                        TokenCredential? tokenCredential = TokenCredentialProvider.Provide(configuration);
                        if (tokenCredential is null)
                        {
                            throw new InvalidOperationException("TokenCredential is not provided");
                        }

                        return new BlobServiceClient(blobUri, tokenCredential);
                    }
                );
            }
        );

        return services;
    }

    private static IServiceCollection AddHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddAzureBlobStorage(
                optionsFactory: serviceProvider =>
                {
                    AzureStorageAccountOptions options =
                        serviceProvider.GetRequiredService<IOptions<AzureStorageAccountOptions>>().Value;

                    return new AzureBlobStorageHealthCheckOptions
                    {
                        ContainerName = options.InputContainerName
                    };
                },
                name: "azureblobstorage:inputcontainer"
            )
            .AddAzureBlobStorage(
                optionsFactory: serviceProvider =>
                {
                    AzureStorageAccountOptions options =
                        serviceProvider.GetRequiredService<IOptions<AzureStorageAccountOptions>>().Value;

                    return new AzureBlobStorageHealthCheckOptions
                    {
                        ContainerName = options.OutputContainerName
                    };
                },
                name: "azureblobstorage:outputcontainer"
            );

        return services;
    }

    private static IServiceCollection AddJobsQueue(this IServiceCollection services)
    {
        services.AddServiceBusQueueSender<IJobsQueueSender, JobsQueueSender, JobMessage, AzureServiceBusOptions>();
        services.AddQueueCreator<AzureServiceBusOptions>();

        return services;
    }
}
