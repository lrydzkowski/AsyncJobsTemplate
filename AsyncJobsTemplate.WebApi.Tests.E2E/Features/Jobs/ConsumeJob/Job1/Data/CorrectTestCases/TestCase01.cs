using System.Text.Json;
using AsyncJobsTemplate.Core.Jobs.Job1;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job1.Data.CorrectTestCases;

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
                Key6 = new DateTimeOffset(2025, 1, 9, 10, 11, 0, TimeSpan.Zero)
            }
        };
        string existingInputDataSerialized = JsonSerializer.Serialize(
            existingInputData,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        return new TestCaseData
        {
            TestCaseId = 1,
            JobId = new Guid("EE1BBD3A-385D-4FFB-AE06-CD870BF12C26"),
            Data = new BaseTestCaseData
            {
                Db = new DbTestCaseData
                {
                    Jobs =
                    [
                        new JobEntity
                        {
                            JobId = new Guid("EE1BBD3A-385D-4FFB-AE06-CD870BF12C26"),
                            JobCategoryName = Job1Handler.Name,
                            Status = "Created",
                            InputData = existingInputDataSerialized,
                            CreatedAt = new DateTimeOffset(2025, 1, 1, 10, 0, 0, TimeSpan.Zero),
                            UserEmail = "test@asyncjobstemplate.com"
                        }
                    ]
                }
            }
        };
    }
}
