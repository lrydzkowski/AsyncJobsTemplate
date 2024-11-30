namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJob.Data.TestCases;

internal static class TestCase01
{
    public static TestCaseData Get()
    {
        // Incorrect GUID as JobId
        return new TestCaseData
        {
            TestCaseId = 1,
            JobId = "30d49143-3fee-4a5d-a150-1e18acb5f57d1"
        };
    }
}
