namespace AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Models;

public class SenderResult
{
    public List<string> MessageIds { get; init; } = [];
}
