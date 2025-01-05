namespace AsyncJobsTemplate.Core.Common.Models;

public class Job
{
    public Guid JobId { get; set; }

    public string JobCategoryName { get; set; } = "";

    public JobStatus Status { get; set; } = JobStatus.Created;

    public object? InputData { get; set; }

    public string? InputFileReference { get; set; }

    public object? OutputData { get; set; }

    public string? OutputFileReference { get; set; }

    public List<JobError> Errors { get; set; } = [];

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? LastUpdatedAt { get; set; }
}
