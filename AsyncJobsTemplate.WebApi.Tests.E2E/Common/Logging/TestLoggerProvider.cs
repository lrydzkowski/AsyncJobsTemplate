using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;

internal class TestLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentQueue<string> _logMessages;

    public TestLoggerProvider(ConcurrentQueue<string> logMessages)
    {
        _logMessages = logMessages;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new TestLogger(_logMessages);
    }

    public void Dispose()
    {
    }
}
