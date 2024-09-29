namespace AsyncJobsTemplate.Core.Models;

internal interface IProcess
{
    List<JobError> Errors { get; init; }

    public bool HasErrors { get; }
}
