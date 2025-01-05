using System.Net;
using System.Reflection;
using AsyncJobsTemplate.Shared.Services;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCollections;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;
using FluentAssertions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Xunit.Abstractions;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Api;

[Collection(ApiTestsCollection.CollectionName)]
[Trait(TestConstants.Category, ApiTestsCollection.CollectionName)]
public class ApiTests
{
    private readonly EndpointDataSource _endpointDataSource;
    private readonly ITestOutputHelper _output;
    private readonly WebApiFactory _webApiFactory;

    public ApiTests(ITestOutputHelper output, WebApiFactory webApiFactory)
    {
        _output = output;
        _webApiFactory = webApiFactory;
        _endpointDataSource = _webApiFactory.Services.GetRequiredService<EndpointDataSource>();
    }

    [Fact]
    public async Task SendRequest_ShouldReturn401_WhenNoAccessToken()
    {
        IReadOnlyList<EndpointInfo> endpointsInfo = EndpointHelpers.GetEndpointsWithAuth(_endpointDataSource);
        foreach (EndpointInfo endpointInfo in endpointsInfo)
        {
            _output.WriteLine($"{endpointInfo.HttpMethod} {endpointInfo.Path}");

            using HttpRequestMessage requestMessage = new(endpointInfo.HttpMethod, endpointInfo.Path);
            using HttpResponseMessage responseMessage = await _webApiFactory.CreateClient().SendAsync(requestMessage);

            responseMessage.StatusCode.Should()
                ?.Be(
                    HttpStatusCode.Unauthorized,
                    $"endpoint {endpointInfo.HttpMethod} {endpointInfo.Path} should require authorization"
                );
        }
    }

    [Fact]
    public async Task SendRequest_ShouldReturn401_WhenExpiredAccessToken()
    {
        IReadOnlyList<EndpointInfo> endpointsInfo = EndpointHelpers.GetEndpointsWithAuth(_endpointDataSource);
        foreach (EndpointInfo endpointInfo in endpointsInfo)
        {
            _output.WriteLine($"{endpointInfo.HttpMethod} {endpointInfo.Path}");

            string accessToken = EmbeddedFile.GetContent(
                "Api/Assets/expired_access_token.txt",
                Assembly.GetExecutingAssembly()
            );
            using HttpRequestMessage requestMessage = new(endpointInfo.HttpMethod, endpointInfo.Path);
            requestMessage.Headers.Add(HeaderNames.Authorization, $"Bearer {accessToken}");

            using HttpResponseMessage responseMessage = await _webApiFactory.CreateClient().SendAsync(requestMessage);

            responseMessage.StatusCode.Should()
                ?.Be(
                    HttpStatusCode.Unauthorized,
                    $"endpoint {endpointInfo.HttpMethod} {endpointInfo.Path} should require authorization"
                );
        }
    }

    [Fact]
    public async Task SendRequest_ShouldReturn401_WhenWrongSignatureInAccessToken()
    {
        IReadOnlyList<EndpointInfo> endpointsInfo = EndpointHelpers.GetEndpointsWithAuth(_endpointDataSource);
        foreach (EndpointInfo endpointInfo in endpointsInfo)
        {
            _output.WriteLine($"{endpointInfo.HttpMethod} {endpointInfo.Path}");

            string accessToken = EmbeddedFile.GetContent(
                "Api/Assets/wrong_signature_access_token.txt",
                Assembly.GetExecutingAssembly()
            );
            using HttpRequestMessage requestMessage = new(endpointInfo.HttpMethod, endpointInfo.Path);
            requestMessage.Headers.Add(HeaderNames.Authorization, $"Bearer {accessToken}");

            using HttpResponseMessage responseMessage = await _webApiFactory.CreateClient().SendAsync(requestMessage);

            responseMessage.StatusCode.Should()
                ?.Be(
                    HttpStatusCode.Unauthorized,
                    $"endpoint {endpointInfo.HttpMethod} {endpointInfo.Path} should require authorization"
                );
        }
    }
}
