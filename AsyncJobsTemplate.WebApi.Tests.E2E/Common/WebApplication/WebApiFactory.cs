using AsyncJobsTemplate.Infrastructure.Azure;
using AsyncJobsTemplate.Infrastructure.Azure.Options;
using AsyncJobsTemplate.Infrastructure.Db;
using AsyncJobsTemplate.Infrastructure.Db.Options;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
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
        VerifySettings.DontIgnoreEmptyCollections();
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
        builder.ConfigureHostConfiguration(configBuilder =>
            {
                SetDatabaseConnectionString(configBuilder);
                DisableJobsQueueConsumer(configBuilder);
                SetAzureStorageAccountConnectionString(configBuilder);
            }
        );

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        DisableLogging(builder);
        DisableUserSecrets(builder);
    }

    private void DisableLogging(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddProvider(new TestLoggerProvider(LogMessages.Get()));
            }
        );
    }

    private static void DisableUserSecrets(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                IConfigurationSource? userSecretsSource =
                    configBuilder.Sources.FirstOrDefault(source => source is JsonConfigurationSource
                        {
                            Path: "secrets.json"
                        }
                    );
                if (userSecretsSource is not null)
                {
                    configBuilder.Sources.Remove(userSecretsSource);
                }
            }
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

    private void DisableJobsQueueConsumer(IConfigurationBuilder configBuilder)
    {
        configBuilder.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                [$"{AzureServiceBusOptions.Position}:{nameof(AzureServiceBusOptions.IsEnabled)}"] = false.ToString()
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
