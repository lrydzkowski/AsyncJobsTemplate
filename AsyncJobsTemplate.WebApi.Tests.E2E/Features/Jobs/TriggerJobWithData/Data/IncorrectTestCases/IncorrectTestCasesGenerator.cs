using Testcontainers.MsSql;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithData.Data.IncorrectTestCases;

internal static class IncorrectTestCasesGenerator
{
    public static IEnumerable<TestCaseData> Generate(MsSqlContainer dbContainer)
    {
        yield return TestCase01.Get(dbContainer);
    }
}
