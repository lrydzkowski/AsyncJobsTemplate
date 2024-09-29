using AsyncJobsTemplate.Core.Jobs;
using AsyncJobsTemplate.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AsyncJobsTemplate.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        return services.AddMediatR().AddJobs().AddServices();
    }

    public static IServiceCollection AddOptionsType<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string configurationPosition
    ) where TOptions : class
    {
        services.AddOptions<TOptions>().Bind(configuration.GetSection(configurationPosition));

        return services;
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        return services.AddMediatR(
            cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly)
        );
    }

    private static IServiceCollection AddJobs(this IServiceCollection services)
    {
        return services.AddKeyedScoped<IJob, Job1Example>(Job1Example.Name)
            .AddKeyedScoped<IJob, Job2Example>(Job2Example.Name);
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
    }
}
