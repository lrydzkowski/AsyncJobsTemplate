using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.DownloadJobFile.Data;

internal class TestCaseData : ITestCaseData
{
    public string JobId { get; init; } = "";

    public int TestCaseId { get; init; }

    public BaseTestCaseData Data { get; init; } = new();
}