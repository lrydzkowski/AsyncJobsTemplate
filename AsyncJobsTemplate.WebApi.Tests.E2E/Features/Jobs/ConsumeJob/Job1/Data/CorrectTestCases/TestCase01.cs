using AsyncJobsTemplate.Core.Jobs.Job1;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job1.Data.CorrectTestCases;

internal static class TestCase01
{
    public static TestCaseData Get()
    {
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
                            InputData = """
                                        {
                                          "key1": "Value1",
                                          "key2": {
                                            "key3": "Value3",
                                            "key4": 4,
                                            "key5": true,
                                            "key6": "2025-01-09T10:11:00+00:00"
                                          }
                                        }
                                        """,
                            CreatedAt = new DateTimeOffset(2025, 1, 1, 10, 0, 0, TimeSpan.Zero),
                            UserEmail = "test@asyncjobstemplate.com"
                        }
                    ]
                }
            }
        };
    }
}
