using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Core.Jobs;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJobs.Data.CorrectTestCases;

internal static class TestCase03
{
    public static TestCaseData Get()
    {
        return new TestCaseData
        {
            TestCaseId = 3,
            Page = 2,
            PageSize = 1,
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
                        },
                        new JobEntity
                        {
                            JobId = Guid.Parse("7537befd-c32e-45eb-94a9-104b0f6e6b11"),
                            JobCategoryName = Job2Handler.Name,
                            Status = JobStatus.Running.ToString(),
                            InputData = "{\"key\":\"value\"}",
                            InputFileReference = "283e9f39-d018-4562-8be7-be10e0762644",
                            OutputData = null,
                            OutputFileReference = null,
                            Errors = null,
                            CreatedAtUtc = new DateTime(2024, 12, 1),
                            LastUpdatedAtUtc = null
                        },
                        new JobEntity
                        {
                            JobId = Guid.Parse("599d4f6f-a97b-4d21-bcc5-cfad5e94f385"),
                            JobCategoryName = Job2Handler.Name,
                            Status = JobStatus.Running.ToString(),
                            InputData = "{\"key1\":\"value1\"}",
                            InputFileReference = "543294f2-ada9-4814-aa43-218bf5b9e693",
                            OutputData = "{\"key2\":\"value2\"}",
                            OutputFileReference = "b49c88c0-7cae-4c2d-a6d4-68645bab0717",
                            Errors =
                                "[{\"message\":\"Test message\",\"errorCode\":\"error1\",\"exceptionMessage\":\"Exception message\"},{\"message\":\"Test message 2\",\"errorCode\":\"error2\",\"exceptionMessage\":\"Exception message 2\"}]",
                            CreatedAtUtc = new DateTime(2024, 12, 1),
                            LastUpdatedAtUtc = new DateTime(2024, 12, 2)
                        }
                    ]
                }
            }
        };
    }
}
