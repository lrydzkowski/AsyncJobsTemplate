using System.Text.Json;
using AsyncJobsTemplate.Core.Jobs.Job2;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job2.Data.CorrectTestCases;

internal static class TestCase01
{
    public static TestCaseData Get()
    {
        var existingInputData = new
        {
            Key1 = "Value1",
            Key2 = new
            {
                Key3 = "Value3",
                Key4 = 4,
                Key5 = true,
                Key6 = new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc)
            }
        };
        string existingInputDataSerialized = JsonSerializer.Serialize(
            existingInputData,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        return new TestCaseData
        {
            TestCaseId = 1,
            JobId = new Guid("F5EA43A0-C675-4259-B13E-636DFAE6BBCC"),
            Data = new BaseTestCaseData
            {
                Db = new DbTestCaseData
                {
                    Jobs =
                    [
                        new JobEntity
                        {
                            JobId = new Guid("F5EA43A0-C675-4259-B13E-636DFAE6BBCC"),
                            JobCategoryName = Job2Handler.Name,
                            Status = "Created",
                            InputData = existingInputDataSerialized,
                            CreatedAt = new DateTimeOffset(2025, 1, 1, 10, 0, 0, TimeSpan.Zero)
                        }
                    ]
                }
            }
        };
    }
}
