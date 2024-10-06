using System.Net.Mime;
using System.Text;
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

    public string Description { get; } = "A simple job that generates file as output";

    public async Task<JobExecutionOutput> RunJobAsync(JobExecutionInput input, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start processing Job2 - JobId = {JobId}", input.JobId);

        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);

        JobExecutionOutput output = new()
        {
            OutputFile = GetFile()
        };

        _logger.LogInformation("Stop processing Job2 - JobId = {JobId}", input.JobId);

        return output;
    }

    private JobFile GetFile()
    {
        string text = "This is an example of a text file.";
        Stream content = new MemoryStream(Encoding.UTF8.GetBytes(text));

        return new JobFile
        {
            Content = content,
            FileName = "test.txt",
            ContentType = MediaTypeNames.Text.Plain
        };
    }
}
