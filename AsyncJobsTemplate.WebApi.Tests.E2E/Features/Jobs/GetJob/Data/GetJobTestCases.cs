using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Core.Jobs;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJob.Data;

internal static class GetJobTestCases
{
    public static IEnumerable<TestCaseData> Get()
    {
        // Incorrect GUID as JobId
        yield return new TestCaseData
        {
            TestCaseId = 1,
            JobId = "30d49143-3fee-4a5d-a150-1e18acb5f57d1"
        };

        // Job doesn't exist
        yield return new TestCaseData
        {
            TestCaseId = 2,
            JobId = "17ba1bd4-2569-46be-a692-9c2dedfcfedc"
        };

        // Job exists
        yield return new TestCaseData
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
                            OutputData = null,
                            OutputFileReference = null,
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
