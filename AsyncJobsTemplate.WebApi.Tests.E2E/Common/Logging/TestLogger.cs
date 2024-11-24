using System.Collections.Concurrent;
using AsyncJobsTemplate.Core.Commands.RunJob;
using AsyncJobsTemplate.Core.Commands.TriggerJob;
using AsyncJobsTemplate.Core.Jobs;
using AsyncJobsTemplate.Core.Queries.GetJob;
using Microsoft.Extensions.Logging;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;

internal class TestLogger : ILogger
{
    private readonly HashSet<string> _allowedCategories =
    [
        typeof(TriggerJobCommandHandler).FullName!,
        typeof(RunJobCommandHandler).FullName!,
        typeof(Job1Handler).FullName!,
        typeof(Job2Handler).FullName!,
        typeof(GetJobQueryHandler).FullName!
    ];

    private readonly string _categoryName;
    private readonly ConcurrentQueue<string> _logMessages;

    public TestLogger(ConcurrentQueue<string> logMessages, string categoryName)
    {
        _logMessages = logMessages;
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _allowedCategories.Contains(_categoryName);
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string message = formatter(state, exception);
        _logMessages.Enqueue($"{logLevel}: {message}");
    }
}
