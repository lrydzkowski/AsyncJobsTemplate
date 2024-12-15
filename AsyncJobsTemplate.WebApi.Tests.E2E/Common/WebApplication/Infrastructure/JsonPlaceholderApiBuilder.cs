using System.Net;
using AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi.Dtos;
using AsyncJobsTemplate.Shared.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;

internal static class JsonPlaceholderApiBuilder
{
    public static WebApplicationFactory<Program> MockGetTodo(
        this WebApplicationFactory<Program> webApplicationFactory,
        WireMockServer wireMockServer,
        int todoId,
        GetTodoResponseDto response,
        HttpStatusCode responseStatusCode = HttpStatusCode.OK
    )
    {
        webApplicationFactory = webApplicationFactory.WithCustomOptions(
            new Dictionary<string, string?>
            {
                { "JsonPlaceholder:BaseUrl", wireMockServer.Url + "/" }
            }
        );

        string responseBody = new Serializer().Serialize(response);
        wireMockServer.Given(Request.Create().WithPath($"/todos/{todoId}").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(responseStatusCode).WithBody(responseBody));

        return webApplicationFactory;
    }
}
