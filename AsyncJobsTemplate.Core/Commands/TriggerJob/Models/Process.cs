using System.Text.Json.Nodes;
using AsyncJobsTemplate.Core.Models;
using Microsoft.AspNetCore.Http;

namespace AsyncJobsTemplate.Core.Commands.TriggerJob.Models;

internal class Process : IProcess
{
    public Guid JobId { get; set; }

    public IFormFile? InputFile { get; init; }

    public JsonObject? InputData { get; init; }

    public string? InputFileReference { get; set; }

    public List<JobError> Errors { get; init; } = [];

    public bool HasErrors => Errors.Count > 0;
}
