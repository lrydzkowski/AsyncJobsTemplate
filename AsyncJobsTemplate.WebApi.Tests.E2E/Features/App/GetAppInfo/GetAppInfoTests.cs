using System.Net;
using AsyncJobsTemplate.Shared.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;
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
    private readonly LogMessages _logMessages;
    private readonly VerifySettings _verifySettings;
    private readonly WebApplicationFactory<Program> _webApiFactory;

    public GetAppInfoTests(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory;
        _logMessages = webApiFactory.LogMessages;
        _verifySettings = webApiFactory.VerifySettings;
    }

    [Fact]
    public async Task GetAppInfo_ShouldReturnCorrectData()
    {
        await using TestContextScope contextScope = new(_webApiFactory, _logMessages);

        HttpClient client = _webApiFactory.CreateClient();
        (HttpStatusCode responseStatusCode, string response) = await SendRequestAsync(client);
        TestResultWithData<GetAppInfoTestResult> result = BuildTestResult(
            responseStatusCode,
            response
        );

        await Verify(result, _verifySettings);
    }

    private async Task<(HttpStatusCode responseStatusCode, string response)> SendRequestAsync(HttpClient client)
    {
        using HttpRequestMessage requestMessage = new(HttpMethod.Get, _endpointUrlPath);
        using HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
        string response = await responseMessage.GetResponseMessageAsync();

        return (responseMessage.StatusCode, response);
    }

    private TestResultWithData<GetAppInfoTestResult> BuildTestResult(
        HttpStatusCode responseStatusCode,
        string response
    )
    {
        TestResultWithData<GetAppInfoTestResult> result = new()
        {
            TestCaseId = 1,
            Data = new GetAppInfoTestResult
            {
                StatusCode = responseStatusCode,
                Response = response.PrettifyJson(4),
                LogMessages = _logMessages.GetSerialized(6)
            }
        };

        return result;
    }

    private class GetAppInfoTestResult : HttpTestResult
    {
    }
}
