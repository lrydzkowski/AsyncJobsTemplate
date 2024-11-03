using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using MassTransit;

namespace AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;

internal class JobsQueue : IJobsQueue
{
    private readonly IPublishEndpoint _publishEndpoint;

    public JobsQueue(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task SendMessageAsync(Guid jobId, CancellationToken cancellationToken)
    {
        JobMessage message = new() { JobId = jobId };
        await _publishEndpoint.Publish(message, cancellationToken);
    }
}
