namespace AsyncJobsTemplate.Infrastructure.Azure.Options;

public class AzureServiceBusOptions
{
    public const string Position = "AzureServiceBus";

    public string HostAddress { get; init; } = "";

    public string JobQueueName { get; init; } = "";
}
