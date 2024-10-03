using AsyncJobsTemplate.Core.Models;
using Microsoft.Extensions.Logging;

namespace AsyncJobsTemplate.Core.Jobs;

internal class Job2Handler : IJobHandler
{
    public const string Name = "job2";
    private readonly ILogger<Job2Handler> _logger;

    public Job2Handler(ILogger<Job2Handler> logger)
    {
        _logger = logger;
    }

    public async Task<Job> RunJobAsync(Job job, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start processing Job2 - JobId = {JobId}", job.JobId);

        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        job.OutputData = new { Result = true };

        _logger.LogInformation("Stop processing Job2 - JobId = {JobId}", job.JobId);

        return job;
    }
}
