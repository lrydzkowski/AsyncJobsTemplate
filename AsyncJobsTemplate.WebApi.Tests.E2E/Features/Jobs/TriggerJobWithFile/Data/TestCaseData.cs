using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithFile.Data;

internal class TestCaseData : ITestCaseData
{
    public required string CategoryName { get; init; }

    public string FileToSend { get; init; } = "test_payload.csv";

    public Dictionary<string, string?> CustomOptions { get; init; } = new();

    public int TestCaseId { get; init; }

    public BaseTestCaseData Data { get; init; } = new();
}
