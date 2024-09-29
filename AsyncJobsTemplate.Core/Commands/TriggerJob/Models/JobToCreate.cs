using System.Text.Json.Nodes;

namespace AsyncJobsTemplate.Core.Commands.TriggerJob.Models;

public class JobToCreate
{
    public Guid JobId { get; init; }

    public string JobCategoryName { get; init; } = "";

    public JsonObject? InputData { get; init; }

    public string? InputFileReference { get; init; }
}
