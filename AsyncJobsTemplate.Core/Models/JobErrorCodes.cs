namespace AsyncJobsTemplate.Core.Models;

internal static class JobErrorCodes
{
    public const string SaveFileFailure = nameof(SaveFileFailure);
    public const string CreateJobFailure = nameof(CreateJobFailure);
    public const string SendMessageFailure = nameof(SendMessageFailure);
    public const string SaveErrorsFailure = nameof(SaveErrorsFailure);
}
