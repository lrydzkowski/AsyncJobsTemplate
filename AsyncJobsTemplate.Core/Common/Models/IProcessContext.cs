namespace AsyncJobsTemplate.Core.Common.Models;

internal interface IProcessContext
{
    List<JobError> Errors { get; init; }

    public bool HasErrors { get; }

    public bool HasInternalErrors { get; }
}

internal abstract class ProcessContextBase : IProcessContext
{
    public List<JobError> Errors { get; init; } = [];

    public bool HasErrors => Errors.Count > 0;

    public bool HasInternalErrors => Errors.Any(e => e.Type == ErrorType.Internal);

    public void AddError(string errorCode, string errorMessage, ErrorType errorType = ErrorType.Internal)
    {
        Errors.Add(new JobError { ErrorCode = errorCode, Message = errorMessage, Type = errorType });
    }

    public void AddError(string errorCode, string errorMessage, Exception ex, ErrorType errorType = ErrorType.Internal)
    {
        Errors.Add(new JobError { ErrorCode = errorCode, Message = errorMessage, Exception = ex, Type = errorType });
    }
}
