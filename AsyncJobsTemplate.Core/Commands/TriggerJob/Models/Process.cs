using AsyncJobsTemplate.Core.Models;

namespace AsyncJobsTemplate.Core.Commands.TriggerJob.Models;

internal class Process : IProcess
{
    public Guid JobId { get; set; }

    public string? InputFileReference { get; set; }

    public List<JobError> Errors { get; init; } = [];

    public bool HasErrors => Errors.Count > 0;
}
