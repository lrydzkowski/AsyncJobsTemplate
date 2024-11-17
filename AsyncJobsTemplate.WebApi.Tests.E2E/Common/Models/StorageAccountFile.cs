namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;

internal class StorageAccountFile
{
    public string Name { get; init; } = "";

    public string Content { get; init; } = "";

    public IDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
}
