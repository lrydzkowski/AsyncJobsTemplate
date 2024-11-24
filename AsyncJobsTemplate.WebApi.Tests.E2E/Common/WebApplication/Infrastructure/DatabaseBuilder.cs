using AsyncJobsTemplate.Infrastructure.Db.Options;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;

internal static class DatabaseBuilder
{
    public static WebApplicationFactory<Program> MakeDbConnectionStringIncorrect(
        this WebApplicationFactory<Program> webApiFactory,
        string? currentConnectionString
    )
    {
        if (string.IsNullOrWhiteSpace(currentConnectionString))
        {
            return webApiFactory;
        }

        return webApiFactory.WithCustomOptions(
            new Dictionary<string, string?>
            {
                [$"{AzureSqlOptions.Position}:{nameof(AzureSqlOptions.ConnectionString)}"] =
                    ReplacePassword(currentConnectionString, "123")
            }
        );
    }

    private static string ReplacePassword(string connectionString, string newPassword)
    {
        SqlConnectionStringBuilder? builder = new(connectionString)
        {
            Password = newPassword
        };

        return builder.ToString();
    }
}
