using Testcontainers.Azurite;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job2.Data.IncorrectTestCases;

internal static class IncorrectTestCasesGenerator
{
    public static IEnumerable<TestCaseData> Generate(AzuriteContainer azuriteContainer)
    {
        yield return TestCase01.Get(azuriteContainer);
    }
}
