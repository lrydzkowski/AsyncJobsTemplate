using System.Net.Mime;
using System.Text.Json.Nodes;
using AsyncJobsTemplate.Core.Commands.TriggerJob;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AsyncJobsTemplate.WebApi.Controllers;

[ApiController]
[Route("jobs")]
public class JobsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [SwaggerOperation(Summary = "Trigger a job with JSON payload")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Correct response",
        typeof(TriggerJobResult),
        MediaTypeNames.Application.Json
    )]
    [HttpPost("{jobCategoryName}")]
    public async Task<IActionResult> Trigger(string jobCategoryName, JsonObject? payload)
    {
        TriggerJobResult result = await _mediator.Send(
            new TriggerJobCommand
            {
                Request = new TriggerJobRequest
                {
                    JobCategoryName = jobCategoryName,
                    Data = payload
                }
            }
        );

        return Ok(result);
    }

    [SwaggerOperation(Summary = "Trigger a job with file")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Correct response",
        typeof(TriggerJobResult),
        MediaTypeNames.Application.Json
    )]
    [HttpPost("{jobCategoryName}/file")]
    public async Task<IActionResult> Trigger(string jobCategoryName, IFormFile? file)
    {
        TriggerJobResult result = await _mediator.Send(
            new TriggerJobCommand
            {
                Request = new TriggerJobRequest
                {
                    JobCategoryName = jobCategoryName,
                    File = file
                }
            }
        );

        return Ok(result);
    }
}
