using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Extensions;

internal static class StringExtensions
{
    public static string ReplacePasswordInSqlConnectionString(this string connectionString, string newPassword)
    {
        SqlConnectionStringBuilder? builder = new(connectionString)
        {
            Password = newPassword
        };

        return builder.ToString();
    }

    public static string ReplaceAccountNameInStorageAccountConnectionString(
        this string connectionString,
        string newAccountName
    )
    {
        string pattern = @"(AccountName=)([^;]*)";
        string replacement = $"AccountName={newAccountName}";

        return Regex.Replace(connectionString, pattern, replacement);
    }
}
