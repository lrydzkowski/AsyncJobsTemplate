using AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi.Health;

internal class JsonPlaceholderApiHealthCheck : IHealthCheck
{
    public const string Name = "jsonplaceholderapi";
    private readonly ILogger<JsonPlaceholderApiHealthCheck> _logger;
    private readonly ITodoClient _todoClient;

    public JsonPlaceholderApiHealthCheck(
        ILogger<JsonPlaceholderApiHealthCheck> logger,
        ITodoClient todoClient
    )
    {
        _logger = logger;
        _todoClient = todoClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = new()
    )
    {
        try
        {
            await _todoClient.GetTodoAsync(1, cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error has occurred in testing JsonPlaceholderApi integration");

            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}
