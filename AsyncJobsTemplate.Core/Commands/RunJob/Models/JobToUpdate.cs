using AsyncJobsTemplate.Core.Common.Models;

namespace AsyncJobsTemplate.Core.Commands.RunJob.Models;

public class JobToUpdate
{
    public Guid JobId { get; init; }

    public JobStatus Status { get; init; }

    public object? OutputData { get; init; }

    public string? OutputFileReference { get; init; }

    public List<JobError>? Errors { get; init; }
}
