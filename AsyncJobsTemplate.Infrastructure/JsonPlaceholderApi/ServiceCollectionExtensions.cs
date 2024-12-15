using AsyncJobsTemplate.Core;
using AsyncJobsTemplate.Core.Jobs.Job3.Services;
using AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi.Options;
using AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJsonPlaceholderApiServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services.AddServices()
            .AddOptions(configuration)
            .AddJsonPlaceholderApi();
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddScoped<ITodoClient, TodoClient>()
            .AddScoped<ITodoRepository, TodoRepository>();
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddOptionsType<JsonPlaceholderOptions>(configuration, JsonPlaceholderOptions.Position);
    }

    private static IServiceCollection AddJsonPlaceholderApi(this IServiceCollection services)
    {
        return services.AddJsonPlaceholderApiHttpClient();
    }
}
