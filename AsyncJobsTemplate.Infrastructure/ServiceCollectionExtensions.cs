using AsyncJobsTemplate.Core;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Infrastructure.Azure.Authentication;
using AsyncJobsTemplate.Infrastructure.Azure.Options;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;
using AsyncJobsTemplate.Infrastructure.Azure.StorageAccount;
using AsyncJobsTemplate.Infrastructure.Db;
using AsyncJobsTemplate.Infrastructure.Db.Mappers;
using AsyncJobsTemplate.Infrastructure.Db.Options;
using AsyncJobsTemplate.Infrastructure.Db.Repositories;
using Azure.Core;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using IJobsRepositoryGetJob = AsyncJobsTemplate.Core.Queries.GetJob.Interfaces.IJobsRepository;
using IJobsRepositoryGetJobs = AsyncJobsTemplate.Core.Queries.GetJobs.Interfaces.IJobsRepository;
using IJobsRepositoryRunJob = AsyncJobsTemplate.Core.Commands.RunJob.Interfaces.IJobsRepository;
using IJobsRepositoryTriggerJob = AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces.IJobsRepository;
using IJobsFileStorageRunJob = AsyncJobsTemplate.Core.Commands.RunJob.Interfaces.IJobsFileStorage;
using IJobsFileStorageTriggerJob = AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces.IJobsFileStorage;
using IJobsFileStorageDownloadJobFile = AsyncJobsTemplate.Core.Queries.DownloadJobFile.Interfaces.IJobsFileStorage;

namespace AsyncJobsTemplate.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services.AddServices()
            .AddOptions(configuration)
            .AddAzureBlobServiceClient()
            .AddAppDbContext()
            .AddAuthentication(configuration);
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddScoped<IJobsRepositoryTriggerJob, JobsRepository>()
            .AddScoped<IJobsRepositoryRunJob, JobsRepository>()
            .AddScoped<IJobsRepositoryGetJob, JobsRepository>()
            .AddScoped<IJobsRepositoryGetJobs, JobsRepository>()
            .AddScoped<IJobsQueue, JobsQueue>()
            .AddScoped<IJobsFileStorageRunJob, JobsFileStorage>()
            .AddScoped<IJobsFileStorageTriggerJob, JobsFileStorage>()
            .AddScoped<IJobsFileStorageDownloadJobFile, JobsFileStorage>()
            .AddSingleton<IAccessTokenProvider, AccessTokenProvider>()
            .AddScoped<IJobMapper, JobMapper>();
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddOptionsType<AzureStorageAccountOptions>(configuration, AzureStorageAccountOptions.Position)
            .AddOptionsType<AzureAdOptions>(configuration, AzureAdOptions.Position)
            .AddOptionsType<AzureSqlOptions>(configuration, AzureSqlOptions.Position)
            .AddOptionsType<AzureServiceBusOptions>(configuration, AzureServiceBusOptions.Position);
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
                        if (tokenCredential is null)
                        {
                            throw new InvalidOperationException("TokenCredential is not provided");
                        }

                        BlobServiceClient client = new(blobUri, tokenCredential);

                        return client;
                    }
                );
            }
        );

        return services;
    }

    private static IServiceCollection AddAppDbContext(this IServiceCollection services)
    {
        return services.AddDbContext<AppDbContext>(
            (serviceProvider, options) =>
            {
                AzureSqlOptions azureSqlOptions =
                    serviceProvider.GetRequiredService<IOptions<AzureSqlOptions>>().Value;
                options.UseSqlServer(azureSqlOptions.ConnectionString);
            }
        );
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(configuration);

        return services;
    }
}
