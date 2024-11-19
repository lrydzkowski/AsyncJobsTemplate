namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;

internal class TestResult
{
    public int TestCaseId { get; init; }
}

internal class TestResultWithData<TData> : TestResult
{
    public TData? Data { get; init; }
}
