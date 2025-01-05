using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Extensions;
using Testcontainers.MsSql;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job1.Data.IncorrectTestCases;

internal static class TestCase01
{
    // Incorrect SQL Server connection string.
    public static TestCaseData Get(MsSqlContainer dbContainer)
    {
        return new TestCaseData
        {
            TestCaseId = 1,
            JobId = new Guid("30d49143-3fee-4a5d-a150-1e18acb5f57d"),
            CustomOptions = new Dictionary<string, string?>
            {
                ["SqlServer:ConnectionString"] =
                    dbContainer.GetConnectionString().ReplacePasswordInSqlConnectionString("123")
            },
            UseDatabase = false
        };
    }
}
