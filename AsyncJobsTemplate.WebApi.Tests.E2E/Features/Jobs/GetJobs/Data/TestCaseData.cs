using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJobs.Data;

internal class TestCaseData : ITestCaseData
{
    public int? Page { get; init; }

    public int? PageSize { get; init; }

    public int TestCaseId { get; init; }

    public string UserEmail { get; init; } = "test@asyncjobstemplate.com";

    public BaseTestCaseData Data { get; init; } = new();
}
