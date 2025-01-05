using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithData.Data;

internal class TestCaseData : ITestCaseData
{
    public required string CategoryName { get; init; }

    public object DataToSend { get; init; } = new
    {
        Key1 = "Value1",
        Key2 = new
        {
            Key3 = "Value3",
            Key4 = 4,
            Key5 = true,
            Key6 = DateTime.UtcNow
        }
    };

    public Dictionary<string, string?> CustomOptions { get; init; } = new();

    public int TestCaseId { get; init; }

    public BaseTestCaseData Data { get; init; } = new();
}
