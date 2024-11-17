using AsyncJobsTemplate.Infrastructure.Azure;
using AsyncJobsTemplate.Infrastructure.Azure.Options;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;
using AsyncJobsTemplate.Infrastructure.Db;
using AsyncJobsTemplate.Infrastructure.Db.Options;
using AsyncJobsTemplate.WebApi.Options;
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
    private readonly AzuriteContainer _azuriteContainer =
        new AzuriteBuilder().WithImage("mcr.microsoft.com/azure-storage/azurite:3.33.0")?.Build()!;

    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build()!;

    public WebApiFactory()
    {
        VerifySettings.ScrubInlineDateTimes("yyyy-MM-ddTHH:mm:ss.fffffffZ");
    }

    public WireMockServer WireMockServer { get; } = WireMockServer.Start();
    public VerifySettings VerifySettings { get; } = new();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync()!;
        await _azuriteContainer.StartAsync()!;

        Services.ExecuteDbMigration();
        Services.CreateStorageAccountContainers();
    }

    public new async Task DisposeAsync()
    {
        WireMockServer.Dispose();
        await _dbContainer.DisposeAsync();
        await _azuriteContainer.DisposeAsync();
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
    }

    private void DisableLogging(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders());
    }

    private static void DisableUserSecrets(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(
            (context, configBuilder) =>
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

    private void SetDatabaseConnectionString(IConfigurationBuilder configBuilder)
    {
        configBuilder.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                [$"{AzureSqlOptions.Position}:{nameof(AzureSqlOptions.ConnectionString)}"] =
                    _dbContainer.GetConnectionString()
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
                    _azuriteContainer.GetConnectionString()
            }
        );
    }
}
