namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

internal class BaseTestCaseData
{
    public DbTestCaseData Db { get; init; } = new();

    public StorageAccountTestCaseData StorageAccount { get; init; } = new();

    public JsonPlaceholderApiTestCaseData JsonPlaceholderApi { get; init; } = new();
}
