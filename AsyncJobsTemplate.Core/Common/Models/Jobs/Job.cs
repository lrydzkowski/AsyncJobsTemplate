namespace AsyncJobsTemplate.Core.Common.Models.Jobs;

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

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? LastUpdatedAtUtc { get; set; }
}
