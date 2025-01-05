using AsyncJobsTemplate.Core.Common.Models;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

internal class StorageAccountTestCaseData
{
    public List<JobFileInfo> OutputFiles { get; init; } = [];
}

internal class JobFileInfo
{
    public Guid JobId { get; init; }

    public JobFile File { get; init; } = new();
}
