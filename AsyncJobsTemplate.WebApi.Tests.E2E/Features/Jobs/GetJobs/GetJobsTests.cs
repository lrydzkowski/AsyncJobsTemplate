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
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJobs.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJobs.Data.CorrectTestCases;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJobs.Data.IncorrectTestCases;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJobs;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class GetJobsTests
{
    private readonly string _endpointUrlPath = "/jobs";
    private readonly LogMessages _logMessages;
    private readonly VerifySettings _verifySettings;
    private readonly WebApplicationFactory<Program> _webApiFactory;

    public GetJobsTests(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory.DisableAuth();
        _logMessages = webApiFactory.LogMessages;
        _verifySettings = webApiFactory.VerifySettings;
    }

    [Fact]
    public async Task GetJobs_ShouldReturnCorrectResponse()
    {
        List<GetJobTestResult> results = [];
        foreach (TestCaseData testCase in CorrectTestCasesGenerator.Generate())
        {
            results.Add(await RunAsync(testCase));
        }

        await Verify(results, _verifySettings);
    }

    [Fact]
    public async Task GetJobs_ShouldReturnIncorrectResponse()
    {
        List<GetJobTestResult> results = [];
        foreach (TestCaseData testCase in IncorrectTestCasesGenerator.Generate())
        {
            results.Add(await RunAsync(testCase));
        }

        await Verify(results, _verifySettings);
    }

    private async Task<GetJobTestResult> RunAsync(TestCaseData testCaseData)
    {
        await using TestContextScope contextScope = new(_webApiFactory, _logMessages);

        HttpClient client = (await _webApiFactory.BuildAsync(contextScope, testCaseData)).CreateClient();
        using HttpRequestMessage requestMessage = new(HttpMethod.Get, BuildUrl(testCaseData));
        using HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
        string response = await responseMessage.GetResponseMessageAsync();

        GetJobTestResult result = new()
        {
            TestCaseId = testCaseData.TestCaseId,
            Page = testCaseData.Page,
            PageSize = testCaseData.PageSize,
            JobEntitiesDb = await JobsData.GetJobsAsync(contextScope),
            LogMessages = _logMessages.GetSerialized(6),
            StatusCode = responseMessage.StatusCode,
            Response = response.PrettifyJson(4)
        };

        return result;
    }

    private string BuildUrl(TestCaseData testCaseData)
    {
        string url = _endpointUrlPath;
        List<string> queryParameters = [];
        if (testCaseData.Page.HasValue)
        {
            queryParameters.Add($"page={testCaseData.Page}");
        }

        if (testCaseData.PageSize.HasValue)
        {
            queryParameters.Add($"pageSize={testCaseData.PageSize}");
        }

        if (queryParameters.Any())
        {
            url += $"?{string.Join("&", queryParameters)}";
        }

        return url;
    }

    private class GetJobTestResult : IHttpTestResult
    {
        public int? Page { get; init; }
        public int? PageSize { get; init; }
        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];
        public int TestCaseId { get; init; }
        public string? LogMessages { get; init; }
        public HttpStatusCode StatusCode { get; init; }
        public string? Response { get; init; }
    }
}
