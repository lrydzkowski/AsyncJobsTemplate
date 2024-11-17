using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;
using AsyncJobsTemplate.Shared.Extensions;

namespace AsyncJobsTemplate.WebApi.Options;

public class QueueOptions
{
    public const string Position = "Queue";

    public string Type { get; init; } = "";

    public static bool IsQueueInMemory(IConfiguration configuration)
    {
        return configuration.GetValue<string>($"{Position}:{nameof(Type)}").EqualsIgnoreCase(QueueTypes.InMemory);
    }
}
