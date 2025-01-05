using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job2.Data;

internal class TestCaseData : ITestCaseData
{
    public Guid JobId { get; init; }

    public Dictionary<string, string?> CustomOptions { get; init; } = new();

    public bool UseAzureStorageAccount { get; init; } = true;

    public int TestCaseId { get; init; }

    public string UserEmail { get; init; } = "test@asyncjobstemplate.com";

    public BaseTestCaseData Data { get; init; } = new();
}
