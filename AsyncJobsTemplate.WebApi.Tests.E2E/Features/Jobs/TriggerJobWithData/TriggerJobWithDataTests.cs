using System.Net;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.Shared.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.Db;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCollections;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithData.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithData.Data.CorrectTestCases;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithData.Data.IncorrectTestCases;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.MsSql;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithData;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class TriggerJobWithDataTests
{
    private const string CategoryNamePlaceholder = "{categoryName}";
    private readonly MsSqlContainer _dbContainer;
    private readonly string _endpointUrlPath = $"/jobs/{CategoryNamePlaceholder}";
    private readonly LogMessages _logMessages;
    private readonly VerifySettings _verifySettings;
    private readonly WebApplicationFactory<Program> _webApiFactory;

    public TriggerJobWithDataTests(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory.DisableAuth();
        _logMessages = webApiFactory.LogMessages;
        _verifySettings = webApiFactory.VerifySettings;
        _dbContainer = webApiFactory.DbContainer;
    }

    [Fact]
    public async Task TriggerJobWithData_ShouldReturnCorrectResponse()
    {
        List<TriggerJobWithDataTestResult> results = [];
        foreach (TestCaseData testCase in CorrectTestCasesGenerator.Generate())
        {
            results.Add(await RunAsync(testCase));
        }

        await Verify(results, _verifySettings);
    }

    [Fact]
    public async Task TriggerJobWithData_ShouldReturnIncorrectResponse()
    {
        List<TriggerJobWithDataTestResult> results = [];
        foreach (TestCaseData testCase in IncorrectTestCasesGenerator.Generate(_dbContainer))
        {
            results.Add(await RunAsync(testCase));
        }

        await Verify(results, _verifySettings);
    }

    private async Task<TriggerJobWithDataTestResult> RunAsync(TestCaseData testCase)
    {
        WebApplicationFactory<Program>? webApiFactory = _webApiFactory.WithCustomUserEmail(testCase.UserEmail)
            .WithJobsQueueMock(out IJobsQueue jobsQueue);
        await using TestContextScope contextScope = new(webApiFactory, _logMessages);

        HttpClient client = webApiFactory
            .WithCustomOptions(testCase.CustomOptions)
            .CreateClient();
        using HttpRequestMessage requestMessage = new(HttpMethod.Post, BuildUrl(testCase));
        requestMessage.CreateContent(testCase.DataToSend);
        using HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
        string response = await responseMessage.GetResponseMessageAsync();

        TriggerJobWithDataTestResult result = new()
        {
            JobEntitiesDb = await contextScope.GetJobsAsync(),
            SendMessageCalls = jobsQueue.GetReceivedMethodCalls() ?? [],
            TestCaseId = testCase.TestCaseId,
            StatusCode = responseMessage.StatusCode,
            Response = response.PrettifyJson(4),
            LogMessages = _logMessages.GetSerialized(6)
        };

        return result;
    }

    private string BuildUrl(TestCaseData testCase)
    {
        return _endpointUrlPath.Replace(CategoryNamePlaceholder, testCase.CategoryName);
    }

    private class TriggerJobWithDataTestResult : IHttpTestResult
    {
        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];
        public IReadOnlyList<ReceivedMethodCall> SendMessageCalls { get; init; } = [];
        public int TestCaseId { get; init; }
        public HttpStatusCode StatusCode { get; init; }
        public string? Response { get; init; }
        public string? LogMessages { get; init; }
    }
}
