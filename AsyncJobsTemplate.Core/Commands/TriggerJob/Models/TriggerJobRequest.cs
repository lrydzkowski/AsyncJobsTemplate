using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;

namespace AsyncJobsTemplate.Core.Commands.TriggerJob.Models;

public class TriggerJobRequest
{
    public string? JobCategoryName { get; init; }

    public IFormFile? File { get; init; }

    public JsonObject? Data { get; init; }
}
