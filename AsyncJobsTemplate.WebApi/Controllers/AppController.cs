using AsyncJobsTemplate.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AsyncJobsTemplate.WebApi.Controllers;

[ApiController]
[Route("")]
public class AppController : ControllerBase
{
    [SwaggerOperation(Summary = "Get basic information about the app")]
    [HttpGet]
    public IActionResult GetAppInfo()
    {
        return Ok(
            new GetAppInfoResponse
            {
                Name = "AsyncJobsTemplate",
                Version = "1.0"
            }
        );
    }
}
