using System.Text.Json.Nodes;

namespace AsyncJobsTemplate.Core.Models;

public class Job
{
    public Guid JobId { get; init; }

    public string JobCategoryName { get; init; } = "";

    public JobStatus Status { get; init; } = JobStatus.Created;

    public JsonObject? InputData { get; init; }

    public string? InputFileReference { get; set; }

    public JsonObject? OutputData { get; init; }

    public string? OutputFileReference { get; init; }

    public List<JobError> Errors { get; init; } = [];

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? LastUpdatedAtUtc { get; set; }
}
