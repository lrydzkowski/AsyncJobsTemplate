using System.Net.Mime;
using System.Text;
using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Core.Common.Options;
using AsyncJobsTemplate.Shared.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.Core.Jobs;

internal class Job2Handler : IJobHandler
{
    public const string Name = "job2";

    private readonly ILogger<Job2Handler> _logger;
    private readonly Job2Options _options;
    private readonly ISerializer _serializer;

    public Job2Handler(ILogger<Job2Handler> logger, IOptions<Job2Options> options, ISerializer serializer)
    {
        _logger = logger;
        _options = options.Value;
        _serializer = serializer;
    }

    public string Description => "A simple job that generates a file as the output";

    public async Task<JobExecutionOutput> RunJobAsync(JobExecutionInput input, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start processing Job2 - JobId = {JobId}", input.JobId);

        await Task.Delay(_options.Sleep, cancellationToken);

        JobExecutionOutput output = new()
        {
            OutputFile = GetFile(input.InputData ?? "")
        };

        _logger.LogInformation("Stop processing Job2 - JobId = {JobId}", input.JobId);

        return output;
    }

    private JobFile GetFile(object inputData)
    {
        string text = $"This is an example of a text file. Input data: '{_serializer.Serialize(inputData)}'";
        Stream content = new MemoryStream(Encoding.UTF8.GetBytes(text));

        return new JobFile
        {
            Content = content,
            FileName = "test-output.txt",
            ContentType = MediaTypeNames.Text.Plain
        };
    }
}
