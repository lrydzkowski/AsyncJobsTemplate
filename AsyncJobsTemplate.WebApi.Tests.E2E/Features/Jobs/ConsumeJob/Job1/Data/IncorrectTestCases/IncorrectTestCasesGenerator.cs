using Testcontainers.MsSql;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job1.Data.IncorrectTestCases;

internal static class IncorrectTestCasesGenerator
{
    public static IEnumerable<TestCaseData> Generate(MsSqlContainer dbContainer)
    {
        yield return TestCase01.Get(dbContainer);
        yield return TestCase02.Get();
        yield return TestCase03.Get();
    }
}
