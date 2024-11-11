namespace AsyncJobsTemplate.Core.Common.Models;

public class JobError
{
    public string Message { get; init; } = "";

    public Exception? Exception { get; init; }

    public string ErrorCode { get; init; } = "";
}
