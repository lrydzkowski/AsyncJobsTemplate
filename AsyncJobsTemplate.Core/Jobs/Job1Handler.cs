using AsyncJobsTemplate.Core.Common.Models;
using Microsoft.Extensions.Logging;

namespace AsyncJobsTemplate.Core.Jobs;

internal class Job1Handler : IJobHandler
{
    public const string Name = "job1";

    private readonly ILogger<Job1Handler> _logger;

    public Job1Handler(ILogger<Job1Handler> logger)
    {
        _logger = logger;
    }

    public string Description => "A simple job that generates JSON as the output";

    public async Task<JobExecutionOutput> RunJobAsync(JobExecutionInput input, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start processing Job1 - JobId = {JobId}", input.JobId);

        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        JobExecutionOutput output = new()
        {
            OutputData = new { Result = true }
        };

        _logger.LogInformation("Stop processing Job1 - JobId = {JobId}", input.JobId);

        return output;
    }
}
