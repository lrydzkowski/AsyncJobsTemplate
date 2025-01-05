namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithData.Data.CorrectTestCases;

internal static class TestCase01
{
    public static TestCaseData Get()
    {
        return new TestCaseData
        {
            TestCaseId = 1,
            CategoryName = nameof(TestCase01)
        };
    }
}
