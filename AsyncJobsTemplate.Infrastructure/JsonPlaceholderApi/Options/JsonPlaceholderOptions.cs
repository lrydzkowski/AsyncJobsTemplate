namespace AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi.Options;

internal class JsonPlaceholderOptions
{
    public const string Position = "JsonPlaceholder";

    public string BaseUrl { get; init; } = "";

    public string TodoPath { get; init; } = "";
}
