namespace AsyncJobsTemplate.Infrastructure.Db.Models;

internal class JobError
{
    public string Message { get; init; } = "";

    public string ErrorCode { get; init; } = "";
}
