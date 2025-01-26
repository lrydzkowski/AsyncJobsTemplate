using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Extensions;

internal static class StringExtensions
{
    public static string ReplacePasswordInSqlConnectionString(this string connectionString, string newPassword)
    {
        SqlConnectionStringBuilder builder = new(connectionString)
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

    public static string? AddIndentationToString(this string? str, int indentationLength = 8)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        string indentation = new(' ', indentationLength);
        string[] lines = str.Split(Environment.NewLine);

        return lines.Length < 2 ? str : string.Join(Environment.NewLine, lines.Select(line => indentation + line));
    }

    public static string? PrettyPrintJson(this string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return json;
        }

        using JsonDocument doc = JsonDocument.Parse(json);

        return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
    }
}
