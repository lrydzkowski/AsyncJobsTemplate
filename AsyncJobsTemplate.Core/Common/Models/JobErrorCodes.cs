namespace AsyncJobsTemplate.Core.Common.Models;

internal static class JobErrorCodes
{
    public const string SaveFileFailure = nameof(SaveFileFailure);
    public const string CreateJobFailure = nameof(CreateJobFailure);
    public const string SendMessageFailure = nameof(SendMessageFailure);
    public const string SaveErrorsFailure = nameof(SaveErrorsFailure);
    public const string GetJobFailure = nameof(GetJobFailure);
    public const string GetJobTypeFailure = nameof(GetJobTypeFailure);
    public const string RunJobFailure = nameof(RunJobFailure);
    public const string SaveJobStatusFailure = nameof(SaveJobStatusFailure);
}
