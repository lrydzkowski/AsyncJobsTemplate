using System.Text.Json;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job1.Data.IncorrectTestCases;

internal static class TestCase02
{
    // Job doesn't exist.
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
                Key6 = new DateTimeOffset(2025, 1, 1, 10, 0, 0, TimeSpan.Zero)
            }
        };
        string existingInputDataSerialized = JsonSerializer.Serialize(
            existingInputData,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        return new TestCaseData
        {
            TestCaseId = 2,
            JobId = new Guid("A8A3B1AE-5E26-4189-8B88-CF19110341AF"),
            Data = new BaseTestCaseData
            {
                Db = new DbTestCaseData
                {
                    Jobs =
                    [
                        new JobEntity
                        {
                            JobId = new Guid("EE1BBD3A-385D-4FFB-AE06-CD870BF12C26"),
                            JobCategoryName = nameof(CorrectTestCases.TestCase01),
                            Status = "Created",
                            InputData = existingInputDataSerialized,
                            CreatedAtUtc = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc)
                        }
                    ]
                }
            }
        };
    }
}
