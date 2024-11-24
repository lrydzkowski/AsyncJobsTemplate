using System.Text.RegularExpressions;
using AsyncJobsTemplate.Infrastructure.Azure.Options;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;

internal static class StorageAccountBuilder
{
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

    private static string ReplaceAccountName(string connectionString, string newAccountName)
    {
        string pattern = @"(AccountName=)([^;]*)";
        string replacement = $"AccountName={newAccountName}";

        return Regex.Replace(connectionString, pattern, replacement);
    }
}
