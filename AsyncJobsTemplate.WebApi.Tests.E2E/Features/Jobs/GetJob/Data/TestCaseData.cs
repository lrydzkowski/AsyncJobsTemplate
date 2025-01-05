using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJob.Data;

internal class TestCaseData : ITestCaseData
{
    public string JobId { get; init; } = "";

    public int TestCaseId { get; init; }

    public string UserEmail { get; init; } = "test@asyncjobstemplate.com";

    public BaseTestCaseData Data { get; init; } = new();
}
