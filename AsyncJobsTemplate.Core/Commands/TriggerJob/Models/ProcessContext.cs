using AsyncJobsTemplate.Core.Common.Models;
using Microsoft.AspNetCore.Http;

namespace AsyncJobsTemplate.Core.Commands.TriggerJob.Models;

internal class ProcessContext : ProcessContextBase
{
    public Guid JobId { get; init; }

    public string JobCategoryName { get; init; } = "";

    public IFormFile? InputFile { get; init; }

    public object? InputData { get; init; }

    public string? InputFileReference { get; set; }
}
