using AsyncJobsTemplate.Core.Jobs;
using AsyncJobsTemplate.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AsyncJobsTemplate.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        return services.AddMediatR().AddJobs().AddServices();
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(
            cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly)
        );

        return services;
    }

    private static IServiceCollection AddJobs(this IServiceCollection services)
    {
        services.AddKeyedScoped<IJob, Job1Example>(Job1Example.Name)
            .AddKeyedScoped<IJob, Job2Example>(Job2Example.Name);

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
