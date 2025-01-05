using AsyncJobsTemplate.Core.Common.Options;
using AsyncJobsTemplate.Core.Jobs;
using AsyncJobsTemplate.Core.Jobs.Job1;
using AsyncJobsTemplate.Core.Jobs.Job2;
using AsyncJobsTemplate.Core.Jobs.Job3;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AsyncJobsTemplate.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddMediatR().AddJobs().AddFluentValidation().AddOptions(configuration);
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
        return services.AddKeyedScoped<IJobHandler, Job1Handler>(Job1Handler.Name)
            .AddKeyedScoped<IJobHandler, Job2Handler>(Job2Handler.Name)
            .AddKeyedScoped<IJobHandler, Job3Handler>(Job3Handler.Name);
    }

    private static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        return services.AddValidatorsFromAssemblyContaining(
            typeof(ServiceCollectionExtensions),
            includeInternalTypes: true
        )!;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddOptionsType<Job1Options>(configuration, Job1Options.Position)
            .AddOptionsType<Job2Options>(configuration, Job2Options.Position);
    }
}
