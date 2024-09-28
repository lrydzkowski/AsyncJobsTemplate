using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;

namespace AsyncJobsTemplate.Infrastructure.ServiceBus;

internal class JobsQueue : IJobsQueue
{
    public Task SendMessageAsync(Guid jobId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
