using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Core.Jobs.Job1;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJob.Data.TestCases;

internal static class TestCase03
{
    public static TestCaseData Get()
    {
        // Job exists
        return new TestCaseData
        {
            TestCaseId = 3,
            JobId = "30d49143-3fee-4a5d-a150-1e18acb5f57d",
            Data = new BaseTestCaseData
            {
                Db = new DbTestCaseData
                {
                    Jobs =
                    [
                        new JobEntity
                        {
                            JobId = Guid.Parse("30d49143-3fee-4a5d-a150-1e18acb5f57d"),
                            JobCategoryName = Job1Handler.Name,
                            Status = JobStatus.Created.ToString(),
                            InputData = "{\"key\":\"value\"}",
                            InputFileReference = null,
                            OutputData = "{\"key1\":\"value1\"}",
                            OutputFileReference = "21BFDDB3-70E4-40CF-948B-355E071F4F47",
                            Errors = null,
                            CreatedAtUtc = new DateTime(2024, 12, 1),
                            LastUpdatedAtUtc = null
                        }
                    ]
                }
            }
        };
    }
}
