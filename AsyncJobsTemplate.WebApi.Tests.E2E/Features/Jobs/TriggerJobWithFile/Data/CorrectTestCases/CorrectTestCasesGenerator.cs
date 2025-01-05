namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithFile.Data.CorrectTestCases;

internal static class CorrectTestCasesGenerator
{
    public static IEnumerable<TestCaseData> Generate()
    {
        yield return TestCase01.Get();
    }
}
