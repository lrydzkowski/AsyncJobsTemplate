namespace AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;

public interface IJobsQueueSender
{
    Task SendMessageAsync(Guid jobId, CancellationToken cancellationToken);
}
