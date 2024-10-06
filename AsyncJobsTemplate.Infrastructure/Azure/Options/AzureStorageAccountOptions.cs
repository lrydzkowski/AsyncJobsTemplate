namespace AsyncJobsTemplate.Infrastructure.Azure.Options;

internal class AzureStorageAccountOptions
{
    public const string Position = "AzureStorageAccount";

    public string Name { get; init; } = "";

    public string InputContainerName { get; init; } = "";

    public string OutputContainerName { get; init; } = "";
}
