using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AsyncJobsTemplate.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        return services.ConfigureFluentValidation();
    }

    private static IServiceCollection ConfigureFluentValidation(this IServiceCollection services)
    {
        return services.AddValidatorsFromAssemblyContaining(
            typeof(ServiceCollectionExtensions),
            includeInternalTypes: true
        )!;
    }
}
