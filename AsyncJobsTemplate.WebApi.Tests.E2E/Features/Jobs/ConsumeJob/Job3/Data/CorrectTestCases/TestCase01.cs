using AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi.Dtos;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job3.Data.CorrectTestCases;

internal static class TestCase01
{
    public static TestCaseData Get()
    {
        return new TestCaseData
        {
            TestCaseId = 1,
            JobId = Guid.NewGuid(),
            CategoryName = "job3",
            Data = new BaseTestCaseData
            {
                JsonPlaceholderApi = new JsonPlaceholderApiTestCaseData
                {
                    TodoData = new Dictionary<int, GetTodoResponseDto>
                    {
                        [1] = new()
                        {
                            Id = 1,
                            UserId = 2,
                            Title = "title2",
                            Completed = true
                        }
                    }
                }
            }
        };
    }
}
