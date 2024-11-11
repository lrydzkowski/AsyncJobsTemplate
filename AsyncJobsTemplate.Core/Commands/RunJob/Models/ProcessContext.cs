using AsyncJobsTemplate.Core.Common.Models;

namespace AsyncJobsTemplate.Core.Commands.RunJob.Models;

internal class ProcessContext : ProcessContextBase
{
    public Guid? JobId { get; set; }

    public Job? Job { get; set; }
}
