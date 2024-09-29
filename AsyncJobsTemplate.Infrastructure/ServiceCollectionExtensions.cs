using AsyncJobsTemplate.Core;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Infrastructure.Azure.Authentication;
using AsyncJobsTemplate.Infrastructure.Azure.Options;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;
using AsyncJobsTemplate.Infrastructure.Azure.StorageAccount;
using AsyncJobsTemplate.Infrastructure.Db.Repositories;
using Azure.Core;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using IJobsRepositoryGetJob = AsyncJobsTemplate.Core.Queries.GetJob.Interfaces.IJobsRepository;
using IJobsRepositoryRunJob = AsyncJobsTemplate.Core.Commands.RunJob.Interfaces.IJobsRepository;
using IJobsRepositoryTriggerJob = AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces.IJobsRepository;

namespace AsyncJobsTemplate.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services.AddServices().AddOptions(configuration).AddAzureBlobServiceClient();
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IJobsRepositoryTriggerJob, JobsRepository>()
            .AddScoped<IJobsRepositoryRunJob, JobsRepository>()
            .AddScoped<IJobsRepositoryGetJob, JobsRepository>()
            .AddScoped<IJobsQueue, JobsQueue>()
            .AddScoped<IJobsFileStorage, JobsFileStorage>()
            .AddSingleton<IAccessTokenProvider, AccessTokenProvider>();

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsType<AzureStorageAccountOptions>(configuration, AzureStorageAccountOptions.Position)
            .AddOptionsType<AzureAdOptions>(configuration, AzureAdOptions.Position);

        return services;
    }

    private static IServiceCollection AddAzureBlobServiceClient(this IServiceCollection services)
    {
        services.AddAzureClients(
            azureClientFactoryBuilder =>
            {
                azureClientFactoryBuilder.AddClient<BlobServiceClient, BlobClientOptions>(
                    (_, serviceProvider) =>
                    {
                        AzureStorageAccountOptions options = serviceProvider
                            .GetRequiredService<IOptions<AzureStorageAccountOptions>>()
                            .Value;
                        Uri blobUri = new($"https://{options.Name}.blob.core.windows.net");

                        IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
                        TokenCredential? tokenCredential = TokenCredentialProvider.Provide(configuration);

                        BlobServiceClient client = new(blobUri, tokenCredential);

                        return client;
                    }
                );
            }
        );

        return services;
    }
}
