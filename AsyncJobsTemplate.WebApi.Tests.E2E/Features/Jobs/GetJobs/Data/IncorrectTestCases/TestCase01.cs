using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJobs.Data.IncorrectTestCases;

internal static class TestCase01
{
    public static TestCaseData Get()
    {
        return new TestCaseData
        {
            TestCaseId = 1,
            Page = -1,
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
