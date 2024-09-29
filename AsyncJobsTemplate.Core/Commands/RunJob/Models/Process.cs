using AsyncJobsTemplate.Core.Models;

namespace AsyncJobsTemplate.Core.Commands.RunJob.Models;

internal class Process : IProcess
{
    public Guid? JobId { get; set; }

    public Job? Job { get; set; }

    public bool JobExecutionResult { get; set; }

    public List<JobError> Errors { get; init; } = [];

    public bool HasErrors => Errors.Count > 0;
}
