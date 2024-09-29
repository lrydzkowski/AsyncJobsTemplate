using AsyncJobsTemplate.Core.Models;

namespace AsyncJobsTemplate.Core.Queries.GetJob.Models;

internal class Process : IProcess
{
    public required string JobIdToParse { get; init; }

    public Guid? JobId { get; set; }

    public Job? Job { get; set; }

    public List<JobError> Errors { get; init; } = [];

    public bool HasErrors => Errors.Count > 0;
}
