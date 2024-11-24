using AsyncJobsTemplate.Core.Commands.TriggerJob;
using AsyncJobsTemplate.WebApi.Models;

namespace AsyncJobsTemplate.WebApi.Mappers;

public interface ITriggerJobResponseMapper
{
    TriggerJobResponse Map(TriggerJobResult result);
}

internal class TriggerJobResponseMapper
    : ITriggerJobResponseMapper
{
    public TriggerJobResponse Map(TriggerJobResult result)
    {
        return new TriggerJobResponse
        {
            JobId = result.JobId,
            Result = result.Result
        };
    }
}
