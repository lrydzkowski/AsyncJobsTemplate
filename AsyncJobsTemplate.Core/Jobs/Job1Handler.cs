using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Core.Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.Core.Jobs;

internal class Job1Handler : IJobHandler
{
    public const string Name = "job1";

    private readonly ILogger<Job1Handler> _logger;
    private readonly Job1Options _options;

    public Job1Handler(ILogger<Job1Handler> logger, IOptions<Job1Options> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public string Description => "A simple job that generates JSON as the output";

    public async Task<JobExecutionOutput> RunJobAsync(JobExecutionInput input, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start processing Job1 - JobId = {JobId}", input.JobId);

        await Task.Delay(_options.Sleep, cancellationToken);
        JobExecutionOutput output = new()
        {
            OutputData = new { Result = true }
        };

        _logger.LogInformation("Stop processing Job1 - JobId = {JobId}", input.JobId);

        return output;
    }
}
