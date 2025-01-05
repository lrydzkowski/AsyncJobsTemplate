using AsyncJobsTemplate.Core.Jobs.Job2;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;
using Testcontainers.Azurite;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job2.Data.IncorrectTestCases;

internal static class TestCase01
{
    // Incorrect Azure Storage Account connection string
    public static TestCaseData Get(AzuriteContainer azuriteContainer)
    {
        return new TestCaseData
        {
            TestCaseId = 1,
            JobId = new Guid("6C75377E-FAD6-4128-A22A-EE0C335C754B"),
            CustomOptions = new Dictionary<string, string?>
            {
                ["AzureStorageAccount:ConnectionString"] =
                    azuriteContainer.GetConnectionString().ReplaceAccountNameInStorageAccountConnectionString("123")
            },
            Data = new BaseTestCaseData
            {
                Db = new DbTestCaseData
                {
                    Jobs =
                    [
                        new JobEntity
                        {
                            JobId = new Guid("6C75377E-FAD6-4128-A22A-EE0C335C754B"),
                            JobCategoryName = Job2Handler.Name,
                            Status = "Created",
                            CreatedAt = new DateTimeOffset(2025, 1, 1, 10, 0, 0, TimeSpan.Zero),
                            UserEmail = "test@asyncjobstemplate.com"
                        }
                    ]
                }
            },
            UseAzureStorageAccount = false
        };
    }
}
