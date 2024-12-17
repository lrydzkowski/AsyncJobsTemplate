using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job3.Data;

internal class TestCaseData : ITestCaseData
{
    public Guid JobId { get; init; } = Guid.NewGuid();

    public string CategoryName { get; init; } = "";

    public int TestCaseId { get; init; }

    public BaseTestCaseData Data { get; init; } = new();
}
