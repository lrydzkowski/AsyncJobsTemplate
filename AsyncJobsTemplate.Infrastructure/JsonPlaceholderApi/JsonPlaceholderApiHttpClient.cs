using AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi;

internal static class JsonPlaceholderApiHttpClient
{
    public static IServiceCollection AddJsonPlaceholderApiHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient(
            nameof(JsonPlaceholderApiHttpClient),
            (serviceProvider, client) =>
            {
                IOptions<JsonPlaceholderOptions> options =
                    serviceProvider.GetRequiredService<IOptions<JsonPlaceholderOptions>>();
                client.BaseAddress = new Uri(options.Value.BaseUrl);
            }
        );

        return services;
    }
}
