namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.DownloadJobFile.Data;

internal static class TestCasesGenerator
{
    public static IEnumerable<TestCaseData> Get()
    {
        yield return TestCase01.Get();
        yield return TestCase02.Get();
        yield return TestCase03.Get();
    }
}
