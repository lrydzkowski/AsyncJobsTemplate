using System.Net.Mime;
using AsyncJobsTemplate.Core.Commands.TriggerJob;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Core.Queries.DownloadJobFile;
using AsyncJobsTemplate.Core.Queries.DownloadJobFile.Models;
using AsyncJobsTemplate.Core.Queries.GetJob;
using AsyncJobsTemplate.Core.Queries.GetJob.Models;
using AsyncJobsTemplate.Core.Queries.GetJobs;
using AsyncJobsTemplate.Shared.Models.Lists;
using AsyncJobsTemplate.WebApi.Mappers;
using AsyncJobsTemplate.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AsyncJobsTemplate.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("jobs")]
public class JobsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITriggerJobResponseMapper _triggerJobResponseMapper;

    public JobsController(IMediator mediator, ITriggerJobResponseMapper triggerJobResponseMapper)
    {
        _mediator = mediator;
        _triggerJobResponseMapper = triggerJobResponseMapper;
    }

    [SwaggerOperation(Summary = "Trigger a job with a JSON payload")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Correct response",
        typeof(TriggerJobResponse),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Incorrect request",
        typeof(TriggerJobResponse),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal error",
        typeof(TriggerJobResponse),
        MediaTypeNames.Application.Json
    )]
    [HttpPost("{jobCategoryName}")]
    public async Task<IActionResult> Trigger(string jobCategoryName, object? payload)
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
        TriggerJobResponse response = _triggerJobResponseMapper.Map(result);
        if (!result.Result)
        {
            return result.HasInternalErrors
                ? StatusCode(StatusCodes.Status500InternalServerError, response)
                : BadRequest(response);
        }

        return Ok(response);
    }

    [SwaggerOperation(Summary = "Trigger a job with a file")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Correct response",
        typeof(TriggerJobResponse),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Incorrect request",
        typeof(TriggerJobResponse),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "Internal error",
        typeof(TriggerJobResponse),
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
        TriggerJobResponse response = _triggerJobResponseMapper.Map(result);
        if (!result.Result)
        {
            return result.HasInternalErrors
                ? StatusCode(StatusCodes.Status500InternalServerError, response)
                : BadRequest(response);
        }

        return Ok(response);
    }

    [SwaggerOperation(Summary = "Get a job")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Correct response",
        typeof(GetJobResult),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Job doesn't exist",
        typeof(GetJobResult),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError
    )]
    [HttpGet("{jobId}")]
    public async Task<IActionResult> GetJob(string jobId)
    {
        GetJobResult result = await _mediator.Send(
            new GetJobQuery
            {
                Request = new GetJobRequest
                {
                    JobId = jobId
                }
            }
        );
        if (result.Status == JobStatus.NotExist.ToString())
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [SwaggerOperation(Summary = "Get jobs")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Correct response",
        typeof(GetJobsResult),
        MediaTypeNames.Application.Json
    )]
    [HttpGet]
    public async Task<IActionResult> GetJobs([FromQuery] ListRequest listRequest)
    {
        GetJobsResult result = await _mediator.Send(
            new GetJobsQuery
            {
                Request = listRequest
            }
        );

        return Ok(result);
    }

    [SwaggerOperation(Summary = "Download a job file")]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Job file doesn't exist"
    )]
    [HttpGet("{jobId}/file")]
    public async Task<IActionResult> DownloadJobFile(string jobId)
    {
        DownloadJobFileResult result = await _mediator.Send(
            new DownloadJobFileQuery
            {
                Request = new DownloadJobFileRequest
                {
                    FileReference = jobId
                }
            }
        );
        if (result.File?.Content is null)
        {
            return NotFound(null);
        }

        return File(result.File.Content, result.File.ContentType, result.File.FileName);
    }
}
