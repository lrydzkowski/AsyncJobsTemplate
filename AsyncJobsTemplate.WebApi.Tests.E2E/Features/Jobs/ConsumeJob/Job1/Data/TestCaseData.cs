using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job1.Data;

internal class TestCaseData : ITestCaseData
{
    public Guid JobId { get; init; }

    public Dictionary<string, string?> CustomOptions { get; init; } = new();

    public bool UseDatabase { get; init; } = true;

    public int TestCaseId { get; init; }

    public BaseTestCaseData Data { get; init; } = new();
}
