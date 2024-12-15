using System.Net.Mime;
using System.Text;
using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Core.Jobs.Job3.Models;
using AsyncJobsTemplate.Core.Jobs.Job3.Services;
using AsyncJobsTemplate.Shared.Services;
using Microsoft.Extensions.Logging;

namespace AsyncJobsTemplate.Core.Jobs.Job3;

internal class Job3Handler : IJobHandler
{
    public const string Name = "job3";

    private readonly ILogger<Job3Handler> _logger;
    private readonly ISerializer _serializer;
    private readonly ITodoRepository _todoRepository;

    public Job3Handler(ILogger<Job3Handler> logger, ISerializer serializer, ITodoRepository todoRepository)
    {
        _logger = logger;
        _serializer = serializer;
        _todoRepository = todoRepository;
    }

    public string Description =>
        "A simple job that gets data from jsonplaceholder API and generates a file as the output";

    public async Task<JobExecutionOutput> RunJobAsync(JobExecutionInput input, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start processing Job3 - JobId = {JobId}", input.JobId);

        Todo? todo = await _todoRepository.GetTodoAsync(1, cancellationToken);
        JobExecutionOutput output = new()
        {
            OutputFile = GetFile(todo)
        };

        _logger.LogInformation("Stop processing Job3 - JobId = {JobId}", input.JobId);

        return output;
    }

    private JobFile? GetFile(Todo? todo)
    {
        if (todo is null)
        {
            return null;
        }

        Stream content = new MemoryStream(Encoding.UTF8.GetBytes(_serializer.Serialize(todo)));

        return new JobFile
        {
            Content = content,
            FileName = "todo.json",
            ContentType = MediaTypeNames.Application.Json
        };
    }
}
