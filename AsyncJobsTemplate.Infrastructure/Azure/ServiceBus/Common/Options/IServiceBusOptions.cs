namespace AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Options;

public interface IServiceBusOptions
{
    bool IsEnabled { get; init; }

    string? ConnectionString { get; init; }

    string? HostAddress { get; init; }
}
