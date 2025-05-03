using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Options;

namespace AsyncJobsTemplate.Infrastructure.Azure.Options;

public class AzureServiceBusOptions : IQueueOptions
{
    public const string Position = "AzureServiceBus";

    public bool IsEnabled { get; init; }

    public string? ConnectionString { get; init; }

    public string? HostAddress { get; init; }

    public string QueueName { get; init; } = "";

    public bool CreateQueueOnStartup { get; init; }

    public bool UseMachineNameAsQueueSuffix { get; init; }
}
