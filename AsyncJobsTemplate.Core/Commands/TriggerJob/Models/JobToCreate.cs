using System.Text.Json.Nodes;

namespace AsyncJobsTemplate.Core.Commands.TriggerJob.Models;

public class JobToCreate
{
    public Guid JobId { get; set; }

    public JsonObject? InputData { get; init; }

    public string? InputFileReference { get; set; }
}
