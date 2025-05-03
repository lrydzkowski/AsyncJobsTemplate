using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus.Common.Senders;

namespace AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;

internal class JobsQueueSender : IJobsQueueSender
{
    private readonly IServiceBusSender<JobMessage> _sender;

    public JobsQueueSender(IServiceBusSender<JobMessage> sender)
    {
        _sender = sender;
    }

    public async Task SendMessageAsync(Guid jobId, CancellationToken cancellationToken)
    {
        JobMessage message = new() { JobId = jobId };
        await _sender.SendAsync(message, cancellationToken: cancellationToken);
    }
}
