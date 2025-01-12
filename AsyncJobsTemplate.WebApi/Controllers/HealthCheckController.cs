using System.Net.Mime;
using AsyncJobsTemplate.WebApi.Mappers;
using AsyncJobsTemplate.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;

namespace AsyncJobsTemplate.WebApi.Controllers;

[ApiController]
// [Authorize]
[Route("health-check")]
public class HealthCheckController : ControllerBase
{
    private readonly ICheckHealthResponseMapper _checkHealthResponseMapper;
    private readonly HealthCheckService _healthCheckService;

    public HealthCheckController(
        HealthCheckService healthCheckService,
        ICheckHealthResponseMapper checkHealthResponseMapper
    )
    {
        _healthCheckService = healthCheckService;
        _checkHealthResponseMapper = checkHealthResponseMapper;
    }

    [SwaggerOperation(Summary = "Check health")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        Type = typeof(CheckHealthResponse),
        ContentTypes = [MediaTypeNames.Application.Json]
    )]
    [SwaggerResponse(
        StatusCodes.Status503ServiceUnavailable,
        Type = typeof(CheckHealthResponse),
        ContentTypes = [MediaTypeNames.Application.Json]
    )]
    [HttpGet("")]
    public async Task<IActionResult> CheckHealth()
    {
        HealthReport report = await _healthCheckService.CheckHealthAsync();
        CheckHealthResponse? response = _checkHealthResponseMapper.Map(report);

        return report.Status == HealthStatus.Healthy
            ? Ok(response)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
    }
}
