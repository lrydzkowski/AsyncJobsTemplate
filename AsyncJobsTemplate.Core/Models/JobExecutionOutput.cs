namespace AsyncJobsTemplate.Core.Models;

internal class JobExecutionOutput
{
    public object? OutputData { get; init; }

    public JobFile? OutputFile { get; init; }
}
