namespace AsyncJobsTemplate.Core.Common.Models;

public class JobError
{
    public string Message { get; init; } = "";

    public Exception? Exception { get; init; }

    public string ErrorCode { get; init; } = "";

    public ErrorType Type { get; init; } = ErrorType.Internal;
}

public enum ErrorType
{
    Data,
    Internal
}
