using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Infrastructure.Azure.Options;
using MassTransit;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;

internal class JobsQueue : IJobsQueue
{
    private readonly AzureServiceBusOptions _options;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public JobsQueue(ISendEndpointProvider sendEndpointProvider, IOptions<AzureServiceBusOptions> options)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _options = options.Value;
    }

    public async Task SendMessageAsync(Guid jobId, CancellationToken cancellationToken)
    {
        JobMessage message = new() { JobId = jobId };
        ISendEndpoint endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{_options.JobQueueName}"));
        await endpoint.Send(message, cancellationToken);
    }
}
