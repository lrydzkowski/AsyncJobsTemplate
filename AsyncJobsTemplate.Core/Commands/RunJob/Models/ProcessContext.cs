using AsyncJobsTemplate.Core.Models;

namespace AsyncJobsTemplate.Core.Commands.RunJob.Models;

internal class ProcessContext : ProcessContextBase
{
    public Guid? JobId { get; set; }

    public Job? Job { get; set; }

    public bool JobExecutionResult { get; set; }
}
