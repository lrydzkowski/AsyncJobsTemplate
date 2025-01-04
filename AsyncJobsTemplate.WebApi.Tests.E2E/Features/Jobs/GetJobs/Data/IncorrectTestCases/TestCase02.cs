using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJobs.Data.IncorrectTestCases;

internal static class TestCase02
{
    // Incorrect page size
    public static TestCaseData Get()
    {
        return new TestCaseData
        {
            TestCaseId = 2,
            Page = 1,
            PageSize = 201,
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
