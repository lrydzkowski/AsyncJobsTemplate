namespace AsyncJobsTemplate.WebApi.Models;

public class TriggerJobResponse
{
    public bool Result { get; init; }

    public Guid? JobId { get; init; }
}
