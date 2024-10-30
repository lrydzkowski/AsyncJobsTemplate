using Microsoft.AspNetCore.Http;

namespace AsyncJobsTemplate.Core.Commands.TriggerJob.Models;

public class TriggerJobRequest
{
    public string? JobCategoryName { get; init; }

    public IFormFile? File { get; init; }

    public object? Data { get; init; }
}
