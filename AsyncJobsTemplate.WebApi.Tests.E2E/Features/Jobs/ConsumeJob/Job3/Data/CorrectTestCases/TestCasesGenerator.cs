namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job3.Data.CorrectTestCases;

internal static class TestCasesGenerator
{
    public static IEnumerable<TestCaseData> Get()
    {
        yield return TestCase01.Get();
    }
}
