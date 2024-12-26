using AsyncJobsTemplate.Infrastructure.Azure;
using AsyncJobsTemplate.Infrastructure.Azure.Options;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;
using AsyncJobsTemplate.Infrastructure.Db;
using AsyncJobsTemplate.Infrastructure.Db.Options;
using AsyncJobsTemplate.WebApi.Options;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Testcontainers.Azurite;
using Testcontainers.MsSql;
using WireMock.Server;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;

public class WebApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public WebApiFactory()
    {
        VerifySettings.ScrubInlineDateTimes("yyyy-MM-ddTHH:mm:ss.fffZ");
        VerifySettings.ScrubInlineDateTimes("ddd, dd MMM yyyy HH:mm:ss 'GMT'");
        VerifySettings.ScrubInlineGuids();
        VerifySettings.ScrubInlineSqlServerHost();
    }

    public AzuriteContainer AzuriteContainer { get; } =
        new AzuriteBuilder().WithImage("mcr.microsoft.com/azure-storage/azurite:3.33.0")?.Build()!;

    public MsSqlContainer DbContainer { get; } = new MsSqlBuilder().Build()!;

    public WireMockServer WireMockServer { get; } = WireMockServer.Start();

    public VerifySettings VerifySettings { get; } = new();

    public LogMessages LogMessages { get; } = new();

    public async Task InitializeAsync()
    {
        await DbContainer.StartAsync()!;
        await AzuriteContainer.StartAsync()!;

        Services.ExecuteDbMigration();
        Services.CreateStorageAccountContainers();
    }

    public new async Task DisposeAsync()
    {
        await AzuriteContainer.DisposeAsync();
        await DbContainer.DisposeAsync();
        WireMockServer.Dispose();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(
            configBuilder =>
            {
                SetDatabaseConnectionString(configBuilder);
                EnableQueueInMemory(configBuilder);
                SetAzureStorageAccountConnectionString(configBuilder);
            }
        );

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        DisableLogging(builder);
        DisableUserSecrets(builder);
        ConfigureHttpsPort(builder);
    }

    private void DisableLogging(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(
            loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddProvider(new TestLoggerProvider(LogMessages.Get()));
            }
        );
    }

    private static void DisableUserSecrets(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(
            (_, configBuilder) =>
            {
                IConfigurationSource? userSecretsSource = configBuilder.Sources.FirstOrDefault(
                    source => source is JsonConfigurationSource { Path: "secrets.json" }
                );
                if (userSecretsSource is not null)
                {
                    configBuilder.Sources.Remove(userSecretsSource);
                }
            }
        );
    }

    private static void ConfigureHttpsPort(IWebHostBuilder builder)
    {
        builder.ConfigureServices(
            services => { services.PostConfigure<HttpsRedirectionOptions>(options => { options.HttpsPort = 443; }); }
        );
    }

    private void SetDatabaseConnectionString(IConfigurationBuilder configBuilder)
    {
        configBuilder.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                [$"{SqlServerOptions.Position}:{nameof(SqlServerOptions.ConnectionString)}"] =
                    DbContainer.GetConnectionString()
            }
        );
    }

    private void EnableQueueInMemory(IConfigurationBuilder configBuilder)
    {
        configBuilder.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                [$"{QueueOptions.Position}:{nameof(QueueOptions.Type)}"] = QueueTypes.InMemory
            }
        );
    }

    private void SetAzureStorageAccountConnectionString(IConfigurationBuilder configBuilder)
    {
        configBuilder.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                [$"{AzureStorageAccountOptions.Position}:{nameof(AzureStorageAccountOptions.ConnectionString)}"] =
                    AzuriteContainer.GetConnectionString()
            }
        );
    }
}
