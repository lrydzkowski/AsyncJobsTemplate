using System.Net.Mime;
using AsyncJobsTemplate.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AsyncJobsTemplate.WebApi.Controllers;

[ApiController]
[Route("")]
public class AppController : ControllerBase
{
    [SwaggerOperation(Summary = "Get basic information about the app")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Correct response",
        typeof(GetAppInfoResponse),
        MediaTypeNames.Application.Json
    )]
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetAppInfo()
    {
        return Ok(
            new GetAppInfoResponse
            {
                Name = "AsyncJobsTemplate",
                Version = "1.0.2"
            }
        );
    }
}
