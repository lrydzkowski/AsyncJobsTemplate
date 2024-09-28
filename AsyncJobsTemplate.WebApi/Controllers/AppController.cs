using Microsoft.AspNetCore.Mvc;

namespace AsyncJobsTemplate.WebApi.Controllers;

[ApiController]
[Route("")]
public class AppController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAppInfo()
    {
        return Ok(
            new
            {
                Name = "AsyncJobsTemplate",
                Version = "1.0"
            }
        );
    }
}
