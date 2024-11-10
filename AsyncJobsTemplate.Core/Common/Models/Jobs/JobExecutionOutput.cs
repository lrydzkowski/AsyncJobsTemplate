namespace AsyncJobsTemplate.Core.Common.Models.Jobs;

internal class JobExecutionOutput
{
    public object? OutputData { get; init; }

    public JobFile? OutputFile { get; init; }
}
