using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Extensions;
using Testcontainers.Azurite;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithFile.Data.IncorrectTestCases;

internal static class TestCase02
{
    // Incorrect Azure Storage Account connection string
    public static TestCaseData Get(AzuriteContainer azuriteContainer)
    {
        return new TestCaseData
        {
            TestCaseId = 2,
            CategoryName = nameof(TestCase02),
            CustomOptions = new Dictionary<string, string?>
            {
                ["AzureStorageAccount:ConnectionString"] = azuriteContainer.GetConnectionString()
                    .ReplaceAccountNameInStorageAccountConnectionString("123")
            }
        };
    }
}
