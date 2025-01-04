using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJobs.Data.IncorrectTestCases;

internal static class TestCase03
{
    // Incorrect page size
    public static TestCaseData Get()
    {
        return new TestCaseData
        {
            TestCaseId = 3,
            Page = 1,
            PageSize = -1,
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
