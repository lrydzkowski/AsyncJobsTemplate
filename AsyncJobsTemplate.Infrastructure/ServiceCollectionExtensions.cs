using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;
using AsyncJobsTemplate.Infrastructure.Azure.StorageAccount;
using AsyncJobsTemplate.Infrastructure.Db.Repositories;
using Microsoft.Extensions.DependencyInjection;
using IJobsRepositoryTriggerJob = AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces.IJobsRepository;
using IJobsRepositoryRunJob = AsyncJobsTemplate.Core.Commands.RunJob.Interfaces.IJobsRepository;
using IJobsRepositoryGetJob = AsyncJobsTemplate.Core.Queries.GetJob.Interfaces.IJobsRepository;

namespace AsyncJobsTemplate.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        return services.AddServices();
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IJobsRepositoryTriggerJob, JobsRepository>()
            .AddScoped<IJobsRepositoryRunJob, JobsRepository>()
            .AddScoped<IJobsRepositoryGetJob, JobsRepository>()
            .AddScoped<IJobsQueue, JobsQueue>()
            .AddScoped<IJobsFileStorage, JobsFileStorage>();

        return services;
    }
}
