using System.Text.RegularExpressions;
using AsyncJobsTemplate.Infrastructure.Azure.Options;
using AsyncJobsTemplate.Infrastructure.Db.Options;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJob.DataBuilders;

internal static class WebApiFactoryBuilder
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

    public static WebApplicationFactory<Program> MakeStorageAccountConnectionStringIncorrect(
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
                [$"{AzureStorageAccountOptions.Position}:{nameof(AzureStorageAccountOptions.ConnectionString)}"] =
                    ReplaceAccountName(currentConnectionString, "123")
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

    private static string ReplaceAccountName(string connectionString, string newAccountName)
    {
        string pattern = @"(AccountName=)([^;]*)";
        string replacement = $"AccountName={newAccountName}";

        return Regex.Replace(connectionString, pattern, replacement);
    }
}
