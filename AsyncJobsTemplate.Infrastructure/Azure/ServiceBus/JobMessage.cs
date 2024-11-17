namespace AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;

public class JobMessage
{
    public required Guid JobId { get; init; }
}
