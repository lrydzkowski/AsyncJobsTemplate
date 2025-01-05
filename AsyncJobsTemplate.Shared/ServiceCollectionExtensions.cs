using AsyncJobsTemplate.Shared.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AsyncJobsTemplate.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        return services.AddFluentValidation().AddServices();
    }

    private static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        return services.AddValidatorsFromAssemblyContaining(
            typeof(ServiceCollectionExtensions),
            includeInternalTypes: true
        )!;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddSingleton<IDateTimeOffsetProvider, DateTimeOffsetProvider>()
            .AddSingleton<ISerializer, Serializer>();
    }
}
