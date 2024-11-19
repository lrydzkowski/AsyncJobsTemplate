using System.Collections.Concurrent;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;

public class LogMessages
{
    private readonly ConcurrentQueue<string> _messages = new();

    public void Clear()
    {
        _messages.Clear();
    }

    public ConcurrentQueue<string> Get()
    {
        return _messages;
    }

    public string GetSerialized(int indentSpaces = 0)
    {
        string indentation = new(' ', indentSpaces);

        return string.Join(Environment.NewLine, _messages.Select(message => indentation + message));
    }
}
