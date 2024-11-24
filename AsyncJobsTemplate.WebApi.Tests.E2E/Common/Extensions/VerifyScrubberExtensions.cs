using System.Text.RegularExpressions;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Extensions;

internal static class VerifyScrubberExtensions
{
    public static void ScrubInlineSqlServerHost(this VerifySettings settings)
    {
        settings.AddScrubber(
            input =>
            {
                string pattern = @"\b(\d{1,3}\.){3}\d{1,3},\d+\b";
                string replacement = "sqlserver_host:port";

                string original = input.ToString();
                string updated = Regex.Replace(original, pattern, replacement);
                input.Clear();
                input.Append(updated);
            }
        );
    }
}
