namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

internal interface ITestCaseData
{
    int TestCaseId { get; init; }

    BaseTestCaseData Data { get; init; }
}
