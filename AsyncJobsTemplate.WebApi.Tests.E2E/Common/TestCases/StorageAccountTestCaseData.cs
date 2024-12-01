using AsyncJobsTemplate.Core.Common.Models;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

internal class StorageAccountTestCaseData
{
    public List<JobFileInfo> OutputFiles { get; set; } = [];
}

internal class JobFileInfo
{
    public Guid JobId { get; set; }

    public JobFile File { get; set; } = new();
}
