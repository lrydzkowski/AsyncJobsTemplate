using System.Net;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;

internal abstract class TestResponse
{
    public HttpStatusCode StatusCode { get; init; }
}

internal abstract class TestResponseWithData<TResponseData> : TestResponse
{
    public TResponseData? Response { get; init; }
}
