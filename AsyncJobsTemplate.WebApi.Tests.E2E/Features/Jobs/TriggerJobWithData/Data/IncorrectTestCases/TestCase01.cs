using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Extensions;
using Testcontainers.MsSql;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithData.Data.IncorrectTestCases;

internal static class TestCase01
{
    // Incorrect SQL Server connection string
    public static TestCaseData Get(MsSqlContainer dbContainer)
    {
        return new TestCaseData
        {
            TestCaseId = 1,
            CategoryName = nameof(TestCase01),
            CustomOptions = new Dictionary<string, string?>
            {
                ["SqlServer:ConnectionString"] =
                    dbContainer.GetConnectionString().ReplacePasswordInSqlConnectionString("123")
            }
        };
    }
}
