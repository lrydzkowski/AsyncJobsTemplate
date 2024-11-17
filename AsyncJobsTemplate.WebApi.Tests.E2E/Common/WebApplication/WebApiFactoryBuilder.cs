using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;

internal static class WebApiFactoryBuilder
{
    public static WebApplicationFactory<Program> WithCustomOptions(
        this WebApplicationFactory<Program> webApplicationFactory,
        Dictionary<string, string?> customOptions
    )
    {
        return webApplicationFactory.WithWebHostBuilder(
            builder => builder.ConfigureAppConfiguration(
                (_, configBuilder) => configBuilder.AddInMemoryCollection(customOptions)
            )
        );
    }

    public static WebApplicationFactory<Program> DisableAuth(this WebApplicationFactory<Program> webApiFactory)
    {
        return webApiFactory.DisableAuthentication();
    }

    public static WebApplicationFactory<Program> DisableAuthentication(
        this WebApplicationFactory<Program> webApiFactory
    )
    {
        return webApiFactory.WithWebHostBuilder(
            builder => builder.ConfigureServices(
                services => services.AddSingleton<IAuthorizationHandler, AllowAnonymous>()
            )
        );
    }
}
