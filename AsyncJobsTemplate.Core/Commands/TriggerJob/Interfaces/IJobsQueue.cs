namespace AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;

public interface IJobsQueue
{
    Task SendMessageAsync(Guid jobId, CancellationToken cancellationToken);
}
