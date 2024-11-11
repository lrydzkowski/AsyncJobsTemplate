namespace AsyncJobsTemplate.Shared.Extensions;

public static class StringExtensions
{
    public static bool EqualsIgnoreCase(this string? str, string? value)
    {
        return string.Equals(str, value, StringComparison.InvariantCultureIgnoreCase);
    }
}
