using AsyncJobsTemplate.Shared.Extensions;
using AsyncJobsTemplate.WebApi.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCollections;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.App.GetAppInfo;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class GetAppInfoTests
{
    private readonly string _endpointUrlPath = "/";

    private readonly WebApplicationFactory<Program> _webApiFactory;

    public GetAppInfoTests(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory;
    }

    [Fact]
    public async Task GetAppInfo_ShouldReturnCorrectData()
    {
        HttpRequestMessage requestMessage = new(HttpMethod.Get, _endpointUrlPath);
        HttpResponseMessage responseMessage = await _webApiFactory.CreateClient()
            .SendAsync(requestMessage);
        GetAppInfoResponse? response = await responseMessage.GetResponseAsync<GetAppInfoResponse>();

        RequestResultWithData<GetAppInfoTestResult> result = new()
        {
            TestCaseId = 1,
            StatusCode = responseMessage.StatusCode,
            Data = new GetAppInfoTestResult
            {
                Response = response
            }
        };

        await Verify(result);
    }

    private class GetAppInfoTestResult
    {
        public GetAppInfoResponse? Response { get; init; }
    }
}
