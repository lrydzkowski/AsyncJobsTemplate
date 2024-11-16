namespace AsyncJobsTemplate.WebApi.Tests.E2E.Api;

internal class EndpointInfo
{
    public string Path { get; init; } = "";

    public HttpMethod HttpMethod { get; init; } = HttpMethod.Get;
}
