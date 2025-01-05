using System.Net;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.Shared.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.Db;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCollections;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJob.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJob.Data.CorrectTestCases;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJob.Data.IncorrectTestCases;
using Microsoft.AspNetCore.Mvc.Testing;
using WireMock.Server;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJob;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class GetJobTests
{
    private const string JobIdPlaceholder = "{jobId}";
    private readonly string _endpointUrlPath = "/jobs/{jobId}";
    private readonly LogMessages _logMessages;
    private readonly VerifySettings _verifySettings;
    private readonly WebApplicationFactory<Program> _webApiFactory;
    private readonly WireMockServer _wireMockServer;

    public GetJobTests(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory.DisableAuth();
        _logMessages = webApiFactory.LogMessages;
        _verifySettings = webApiFactory.VerifySettings;
        _wireMockServer = webApiFactory.WireMockServer;
    }

    [Fact]
    public async Task GetJob_ShouldReturnCorrectResponse()
    {
        List<GetJobTestResult> results = [];
        foreach (TestCaseData testCase in CorrectTestCasesGenerator.Generate())
        {
            results.Add(await RunAsync(testCase));
        }

        await Verify(results, _verifySettings);
    }

    [Fact]
    public async Task GetJob_ShouldReturnIncorrectResponse()
    {
        List<GetJobTestResult> results = [];
        foreach (TestCaseData testCase in IncorrectTestCasesGenerator.Generate())
        {
            results.Add(await RunAsync(testCase));
        }

        await Verify(results, _verifySettings);
    }

    private async Task<GetJobTestResult> RunAsync(TestCaseData testCase)
    {
        WebApplicationFactory<Program> webApiFactory = _webApiFactory.WithDependencies(_wireMockServer, testCase);
        await using TestContextScope contextScope = new(webApiFactory, _logMessages);
        await JobsData.CreateJobsAsync(contextScope, testCase);

        HttpClient client = webApiFactory.CreateClient();
        using HttpRequestMessage requestMessage = new(HttpMethod.Get, BuildUrl(testCase));
        using HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
        string response = await responseMessage.GetResponseMessageAsync();

        GetJobTestResult result = new()
        {
            TestCaseId = testCase.TestCaseId,
            JobId = testCase.JobId,
            JobEntitiesDb = await JobsData.GetJobsAsync(contextScope),
            LogMessages = _logMessages.GetSerialized(6),
            StatusCode = responseMessage.StatusCode,
            Response = response.PrettifyJson(4)
        };

        return result;
    }

    private string BuildUrl(TestCaseData testCase)
    {
        return _endpointUrlPath.Replace(JobIdPlaceholder, testCase.JobId);
    }

    private class GetJobTestResult : IHttpTestResult
    {
        public string JobId { get; init; } = "";
        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];
        public int TestCaseId { get; init; }
        public string? LogMessages { get; init; }
        public HttpStatusCode StatusCode { get; init; }
        public string? Response { get; init; }
    }
}
