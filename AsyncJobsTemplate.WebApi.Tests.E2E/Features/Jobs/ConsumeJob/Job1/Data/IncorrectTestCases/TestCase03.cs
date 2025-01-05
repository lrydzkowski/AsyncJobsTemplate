using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job1.Data.IncorrectTestCases;

internal static class TestCase03
{
    public static TestCaseData Get()
    {
        // Job category is not registered.
        return new TestCaseData
        {
            TestCaseId = 3,
            JobId = new Guid("30d49143-3fee-4a5d-a150-1e18acb5f57d"),
            Data = new BaseTestCaseData
            {
                Db = new DbTestCaseData
                {
                    Jobs =
                    [
                        new JobEntity
                        {
                            JobId = Guid.Parse("30d49143-3fee-4a5d-a150-1e18acb5f57d"),
                            JobCategoryName = "non-existing-category",
                            Status = JobStatus.Created.ToString(),
                            InputData = "{\"key\":\"value\"}",
                            InputFileReference = null,
                            OutputData = null,
                            OutputFileReference = null,
                            Errors = null,
                            CreatedAt = new DateTimeOffset(2024, 12, 1, 10, 0, 0, TimeSpan.Zero),
                            LastUpdatedAt = null,
                            UserEmail = "test@asyncjobstemplate.com"
                        }
                    ]
                }
            }
        };
    }
}
