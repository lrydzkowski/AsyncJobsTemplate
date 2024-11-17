namespace AsyncJobsTemplate.Core.Common.Models;

internal class JobExecutionInput
{
    public Guid JobId { get; init; }

    public object? InputData { get; init; }

    public JobFile? InputFile { get; init; }
}
