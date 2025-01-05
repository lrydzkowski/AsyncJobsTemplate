using AsyncJobsTemplate.Core.Jobs.Job3;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job3.Data;

internal class TestCaseData : ITestCaseData
{
    public Guid JobId { get; init; }

    public string CategoryName { get; init; } = Job3Handler.Name;

    public int TestCaseId { get; init; }

    public string UserEmail { get; init; } = "test@asyncjobstemplate.com";

    public BaseTestCaseData Data { get; init; } = new();
}
