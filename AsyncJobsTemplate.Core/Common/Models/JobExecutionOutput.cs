namespace AsyncJobsTemplate.Core.Common.Models;

internal class JobExecutionOutput
{
    public object? OutputData { get; init; }

    public JobFile? OutputFile { get; init; }
}
