using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;

internal class TestLogger : ILogger
{
    private readonly ConcurrentQueue<string> _logMessages;

    public TestLogger(ConcurrentQueue<string> logMessages)
    {
        _logMessages = logMessages;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        string message = formatter(state, exception);
        _logMessages.Enqueue($"{logLevel}: {message}");
    }
}
