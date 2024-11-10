namespace AsyncJobsTemplate.Core.Common.Models.Errors;

public class ErrorInfo
{
    public string PropertyName { get; init; } = "";
    public string ErrorMessage { get; init; } = "";
    public object? AttemptedValue { get; init; }
    public string ErrorCode { get; init; } = "";
}
