namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;

internal class ReceivedMethodCall
{
    public string MethodName { get; init; } = "";

    public IReadOnlyList<object?> ReceivedArguments { get; init; } = new List<object>();
}
