namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

internal class BaseTestCaseData
{
    public DbTestCaseData Db { get; init; } = new();
}
