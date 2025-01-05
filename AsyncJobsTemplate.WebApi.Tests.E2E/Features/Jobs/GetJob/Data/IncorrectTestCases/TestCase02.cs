namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJob.Data.IncorrectTestCases;

internal static class TestCase02
{
    public static TestCaseData Get()
    {
        // Job doesn't exist
        return new TestCaseData
        {
            TestCaseId = 2,
            JobId = "17ba1bd4-2569-46be-a692-9c2dedfcfedc"
        };
    }
}
