using System.Net;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;

public interface ITestResult
{
    int TestCaseId { get; init; }

    string? LogMessages { get; init; }
}

public interface IHttpTestResult : ITestResult
{
    HttpStatusCode StatusCode { get; init; }

    string? Response { get; init; }
}
