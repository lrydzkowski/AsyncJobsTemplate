using AsyncJobsTemplate.Core.Common.Models.Jobs;

namespace AsyncJobsTemplate.Infrastructure.Db.Entities;

internal class JobEntity
{
    public const string TableName = "Job";

    public long? RecId { get; set; }

    public Guid JobId { get; set; }

    public string JobCategoryName { get; set; } = "";

    public string Status { get; set; } = JobStatus.Created.ToString();

    public string? InputData { get; set; }

    public string? InputFileReference { get; set; }

    public string? OutputData { get; set; }

    public string? OutputFileReference { get; set; }

    public string? Errors { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? LastUpdatedAtUtc { get; set; }
}
