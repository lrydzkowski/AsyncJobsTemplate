namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job3.Data.CorrectTestCases;

internal static class CorrectTestCasesGenerator
{
    public static IEnumerable<TestCaseData> Generate()
    {
        yield return TestCase01.Get();
    }
}
