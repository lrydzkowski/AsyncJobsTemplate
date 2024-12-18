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
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJobs.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using CorrectTestCasesGenerator =
    AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJobs.Data.CorrectTestCases.TestCasesGenerator;
using IncorrectTestCasesGenerator =
    AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJobs.Data.IncorrectTestCases.TestCasesGenerator;

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
    public async Task GetJobs_ShouldReturnValidationErrors_WhenIncorrectData()
    {
        List<TestResultWithData<GetJobsTestResult>> results = [];
        foreach (TestCaseData testCaseData in IncorrectTestCasesGenerator.Get())
        {
            await using TestContextScope contextScope = new(_webApiFactory, _logMessages);

            HttpClient client = (await _webApiFactory.BuildAsync(contextScope, testCaseData)).CreateClient();
            (HttpStatusCode responseStatusCode, string response) = await SendRequestAsync(client, testCaseData);
            TestResultWithData<GetJobsTestResult> result = await BuildTestResultAsync(
                testCaseData,
                contextScope,
                responseStatusCode,
                response
            );

            results.Add(result);
        }

        await Verify(results, _verifySettings);
    }

    [Fact]
    public async Task GetJobs_ShouldReturnPaginatedResult_WhenCorrectData()
    {
        List<TestResultWithData<GetJobsTestResult>> results = [];
        foreach (TestCaseData testCaseData in CorrectTestCasesGenerator.Get())
        {
            await using TestContextScope contextScope = new(_webApiFactory, _logMessages);

            HttpClient client = (await _webApiFactory.BuildAsync(contextScope, testCaseData)).CreateClient();
            (HttpStatusCode responseStatusCode, string response) = await SendRequestAsync(client, testCaseData);
            TestResultWithData<GetJobsTestResult> result = await BuildTestResultAsync(
                testCaseData,
                contextScope,
                responseStatusCode,
                response
            );

            results.Add(result);
        }

        await Verify(results, _verifySettings);
    }

    private async Task<(HttpStatusCode responseStatusCode, string response)> SendRequestAsync(
        HttpClient client,
        TestCaseData testCaseData
    )
    {
        string url = BuildUrl(testCaseData);
        using HttpRequestMessage requestMessage = new(HttpMethod.Get, url);
        using HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
        HttpStatusCode responseStatusCode = responseMessage.StatusCode;
        string response = await responseMessage.GetResponseMessageAsync();

        return (responseStatusCode, response);
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

    private async Task<TestResultWithData<GetJobsTestResult>> BuildTestResultAsync(
        TestCaseData testCaseData,
        TestContextScope contextScope,
        HttpStatusCode responseStatusCode,
        string response
    )
    {
        IReadOnlyList<JobEntity> jobEntitiesDb = await JobsData.GetJobsAsync(contextScope);
        TestResultWithData<GetJobsTestResult> result = new()
        {
            TestCaseId = testCaseData.TestCaseId,
            Data = new GetJobsTestResult
            {
                Page = testCaseData.Page,
                PageSize = testCaseData.PageSize,
                StatusCode = responseStatusCode,
                Response = response.PrettifyJson(6),
                JobEntitiesDb = jobEntitiesDb,
                LogMessages = _logMessages.GetSerialized(6)
            }
        };

        return result;
    }

    private class GetJobsTestResult : HttpTestResult
    {
        public int? Page { get; init; }

        public int? PageSize { get; init; }

        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];
    }
}
