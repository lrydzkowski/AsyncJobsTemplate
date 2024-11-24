using System.Net;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;

internal abstract class HttpTestResult
{
    public HttpStatusCode StatusCode { get; init; }

    public string? Response { get; init; }

    public string LogMessages { get; init; } = "";
}
