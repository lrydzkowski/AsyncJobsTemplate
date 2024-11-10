namespace AsyncJobsTemplate.Core.Common.Models.Jobs;

internal interface IProcessContext
{
    List<JobError> Errors { get; init; }

    public bool HasErrors { get; }
}

internal abstract class ProcessContextBase : IProcessContext
{
    public List<JobError> Errors { get; init; } = [];

    public bool HasErrors => Errors.Count > 0;

    public void AddError(string errorCode, string errorMessage)
    {
        Errors.Add(new JobError { ErrorCode = errorCode, Message = errorMessage });
    }

    public void AddError(string errorCode, string errorMessage, Exception ex)
    {
        Errors.Add(new JobError { ErrorCode = errorCode, Message = errorMessage, Exception = ex });
    }
}
