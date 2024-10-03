using AsyncJobsTemplate.Core.Models;
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

    public async Task<Job> RunJobAsync(Job job, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start processing Job1 - JobId = {JobId}", job.JobId);

        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        job.OutputData = new { Result = true };

        _logger.LogInformation("Stop processing Job1 - JobId = {JobId}", job.JobId);

        return job;
    }
}
