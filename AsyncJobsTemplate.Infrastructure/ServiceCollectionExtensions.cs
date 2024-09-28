using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Infrastructure.Db.Repositories;
using AsyncJobsTemplate.Infrastructure.ServiceBus;
using AsyncJobsTemplate.Infrastructure.StorageAccount;
using Microsoft.Extensions.DependencyInjection;

namespace AsyncJobsTemplate.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        return services.AddServices();
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IJobsRepository, JobsRepository>()
            .AddScoped<IJobsQueue, JobsQueue>()
            .AddScoped<IJobsFileStorage, JobsFileStorage>();

        return services;
    }
}
