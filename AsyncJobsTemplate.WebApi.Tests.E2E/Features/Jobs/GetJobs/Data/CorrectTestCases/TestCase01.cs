using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJobs.Data.CorrectTestCases;

internal static class TestCase01
{
    // No jobs
    public static TestCaseData Get()
    {
        return new TestCaseData
        {
            TestCaseId = 1,
            Data = new BaseTestCaseData
            {
                Db = new DbTestCaseData
                {
                    Jobs = []
                }
            }
        };
    }
}
