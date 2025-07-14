using System.Net;
using System.Reflection;
using AsyncJobsTemplate.Shared.Services;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCollections;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Api;

[Collection(ApiTestsCollection.CollectionName)]
[Trait(TestConstants.Category, ApiTestsCollection.CollectionName)]
public class ApiTests
{
    private readonly EndpointDataSource _endpointDataSource;
    private readonly VerifySettings _verifySettings;
    private readonly WebApiFactory _webApiFactory;

    public ApiTests(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory;
        _endpointDataSource = _webApiFactory.Services.GetRequiredService<EndpointDataSource>();
        _verifySettings = webApiFactory.VerifySettings;
    }

    [Fact]
    public async Task SendRequest_ShouldReturn401_WhenNoAccessToken()
    {
        IReadOnlyList<EndpointInfo> endpointsInfo = EndpointHelpers.GetEndpointsWithAuth(_endpointDataSource);
        List<ApiAuth0TestsResult> results = await RunAsync(endpointsInfo);

        await Verify(results, _verifySettings);
    }

    [Fact]
    public async Task SendRequest_ShouldReturn401_WhenExpiredAccessToken()
    {
        IReadOnlyList<EndpointInfo> endpointsInfo = EndpointHelpers.GetEndpointsWithAuth(_endpointDataSource);
        string accessToken = EmbeddedFile.GetContent(
            "Api/Assets/expired_access_token.txt",
            Assembly.GetExecutingAssembly()
        );
        List<ApiAuth0TestsResult> results = await RunAsync(endpointsInfo, accessToken);

        await Verify(results, _verifySettings);
    }

    [Fact]
    public async Task SendRequest_ShouldReturn401_WhenWrongSignatureInAccessToken()
    {
        IReadOnlyList<EndpointInfo> endpointsInfo = EndpointHelpers.GetEndpointsWithAuth(_endpointDataSource);
        string accessToken = EmbeddedFile.GetContent(
            "Api/Assets/wrong_signature_access_token.txt",
            Assembly.GetExecutingAssembly()
        );
        List<ApiAuth0TestsResult> results = await RunAsync(endpointsInfo, accessToken);

        await Verify(results, _verifySettings);
    }

    private async Task<List<ApiAuth0TestsResult>> RunAsync(
        IReadOnlyList<EndpointInfo> endpointsInfo,
        string? accessToken = null
    )
    {
        List<ApiAuth0TestsResult> results = [];
        foreach (EndpointInfo endpointInfo in endpointsInfo)
        {
            using HttpRequestMessage requestMessage = new(endpointInfo.HttpMethod, endpointInfo.Path);
            if (accessToken is not null)
            {
                requestMessage.Headers.Add(HeaderNames.Authorization, $"Bearer {accessToken}");
            }

            using HttpResponseMessage responseMessage = await _webApiFactory
                .CreateClient()
                .SendAsync(requestMessage);

            results.Add(
                new ApiAuth0TestsResult
                {
                    RequestHttpMethod = endpointInfo.HttpMethod,
                    RequestPath = endpointInfo.Path,
                    ResponseStatusCode = responseMessage.StatusCode
                }
            );
        }

        return results;
    }

    private class ApiAuth0TestsResult
    {
        public HttpMethod? RequestHttpMethod { get; init; }

        public string? RequestPath { get; init; }

        public HttpStatusCode ResponseStatusCode { get; init; }
    }
}
