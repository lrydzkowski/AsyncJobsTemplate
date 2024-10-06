namespace AsyncJobsTemplate.Core.Models;

internal class JobExecutionInput
{
    public Guid JobId { get; init; }

    public object? InputData { get; init; }

    public JobFile? InputFile { get; init; }
}
