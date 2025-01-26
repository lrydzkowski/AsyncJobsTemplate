using AsyncJobsTemplate.Core.Jobs.Job3;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job3.Data.CorrectTestCases;

internal static class TestCase01
{
    public static TestCaseData Get()
    {
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
                            JobCategoryName = Job3Handler.Name,
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
