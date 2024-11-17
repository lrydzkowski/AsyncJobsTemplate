using System.Net;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;

internal class RequestResult
{
    public int TestCaseId { get; init; }

    public HttpStatusCode StatusCode { get; init; }
}

internal class RequestResultWithData<TData> : RequestResult
{
    public TData? Data { get; init; }
}
