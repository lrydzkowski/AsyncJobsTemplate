using System.Net.Mime;
using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Core.Jobs.Job1;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.DownloadJobFile.Data.CorrectTestCases;

internal static class TestCase01
{
    public static TestCaseData Get()
    {
        // Return existing file.
        return new TestCaseData
        {
            TestCaseId = 1,
            JobId = "21CB9FB1-8FE9-45B9-89CB-45BAAF90CF5E",
            Data = new BaseTestCaseData
            {
                StorageAccount = new StorageAccountTestCaseData
                {
                    OutputFiles =
                    [
                        new JobFileInfo
                        {
                            JobId = Guid.Parse("21CB9FB1-8FE9-45B9-89CB-45BAAF90CF5E"),
                            File = new JobFile
                            {
                                FileName = "test-file.json",
                                ContentType = MediaTypeNames.Application.Json,
                                Content = new MemoryStream(
                                    "{\"key1\": \"value1\", \"key2\": \"value2\"}"u8.ToArray()
                                )
                            }
                        }
                    ]
                },
                Db = new DbTestCaseData
                {
                    Jobs =
                    [
                        new JobEntity
                        {
                            JobId = Guid.Parse("21CB9FB1-8FE9-45B9-89CB-45BAAF90CF5E"),
                            JobCategoryName = Job1Handler.Name,
                            Status = JobStatus.Created.ToString(),
                            InputData = "{\"key\":\"value\"}",
                            InputFileReference = null,
                            OutputData = null,
                            OutputFileReference = "21CB9FB1-8FE9-45B9-89CB-45BAAF90CF5E",
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
