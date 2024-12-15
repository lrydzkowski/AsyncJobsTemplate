using AsyncJobsTemplate.Core;
using AsyncJobsTemplate.Infrastructure.Db.Mappers;
using AsyncJobsTemplate.Infrastructure.Db.Options;
using AsyncJobsTemplate.Infrastructure.Db.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using IJobsRepositoryGetJob = AsyncJobsTemplate.Core.Queries.GetJob.Interfaces.IJobsRepository;
using IJobsRepositoryGetJobs = AsyncJobsTemplate.Core.Queries.GetJobs.Interfaces.IJobsRepository;
using IJobsRepositoryRunJob = AsyncJobsTemplate.Core.Commands.RunJob.Interfaces.IJobsRepository;
using IJobsRepositoryTriggerJob = AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces.IJobsRepository;

namespace AsyncJobsTemplate.Infrastructure.Db;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDbServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddServices()
            .AddOptions(configuration)
            .AddAppDbContext();
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddScoped<IJobsRepositoryTriggerJob, JobsRepository>()
            .AddScoped<IJobsRepositoryRunJob, JobsRepository>()
            .AddScoped<IJobsRepositoryGetJob, JobsRepository>()
            .AddScoped<IJobsRepositoryGetJobs, JobsRepository>()
            .AddScoped<IJobMapper, JobMapper>();
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddOptionsType<SqlServerOptions>(configuration, SqlServerOptions.Position);
    }

    private static IServiceCollection AddAppDbContext(this IServiceCollection services)
    {
        return services.AddDbContext<AppDbContext>(
            (serviceProvider, options) =>
            {
                SqlServerOptions sqlServerOptions =
                    serviceProvider.GetRequiredService<IOptions<SqlServerOptions>>().Value;
                options.UseSqlServer(sqlServerOptions.ConnectionString);
            }
        );
    }
}
