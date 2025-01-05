using Testcontainers.Azurite;
using Testcontainers.MsSql;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithFile.Data.IncorrectTestCases;

internal static class IncorrectTestCasesGenerator
{
    public static IEnumerable<TestCaseData> Generate(MsSqlContainer dbContainer, AzuriteContainer azuriteContainer)
    {
        yield return TestCase01.Get(dbContainer);
        yield return TestCase02.Get(azuriteContainer);
    }
}
