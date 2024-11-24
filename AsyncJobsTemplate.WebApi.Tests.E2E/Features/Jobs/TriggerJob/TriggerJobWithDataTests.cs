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
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.MsSql;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJob;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class TriggerJobWithDataTests
{
    private readonly MsSqlContainer _dbContainer;
    private readonly string _endpointUrlPath = "/jobs/{categoryName}";
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
    public async Task TriggerJobWithData_ShouldTriggerJobProcessing_WhenCorrectData()
    {
        await using TestContextScope contextScope = new(_webApiFactory, _logMessages);

        HttpClient client = _webApiFactory.MockJobsQueue().CreateClient();
        (HttpResponseMessage responseMessage, string response) = await SendRequestAsync(client);
        TestResultWithData<TriggerJobWithDataTestResult> result = await BuildTestResultAsync(
            contextScope,
            responseMessage,
            response
        );

        await Verify(result, _verifySettings);
    }

    [Fact]
    public async Task TriggerJobWithData_ShouldReturnError_WhenNoAccessToDatabase()
    {
        await using TestContextScope contextScope = new(_webApiFactory, _logMessages);

        HttpClient client = _webApiFactory.MakeDbConnectionStringIncorrect(_dbContainer.GetConnectionString())
            .MockJobsQueue()
            .CreateClient();
        (HttpResponseMessage responseMessage, string response) = await SendRequestAsync(client);
        TestResultWithData<TriggerJobWithDataTestResult> result = await BuildTestResultAsync(
            contextScope,
            responseMessage,
            response
        );

        await Verify(result, _verifySettings);
    }

    private async Task<(HttpResponseMessage responseMessage, string response)> SendRequestAsync(HttpClient client)
    {
        HttpRequestMessage requestMessage = BuildRequestMessage();
        HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
        string response = await responseMessage.GetResponseMessageAsync();

        return (responseMessage, response);
    }

    private HttpRequestMessage BuildRequestMessage()
    {
        string categoryName = nameof(TriggerJobWithData_ShouldTriggerJobProcessing_WhenCorrectData);
        string endpointPath = _endpointUrlPath.Replace("{categoryName}", categoryName);
        var payload = new
        {
            Key1 = "Value1",
            Key2 = new
            {
                Key3 = "Value3",
                Key4 = 4,
                Key5 = true,
                Key6 = DateTime.UtcNow
            }
        };

        HttpRequestMessage requestMessage = new(HttpMethod.Post, endpointPath);
        requestMessage.CreateContent(payload);

        return requestMessage;
    }

    private async Task<TestResultWithData<TriggerJobWithDataTestResult>> BuildTestResultAsync(
        TestContextScope contextScope,
        HttpResponseMessage responseMessage,
        string response
    )
    {
        IReadOnlyList<JobEntity> jobEntitiesDb = await JobsData.GetJobsAsync(contextScope);
        IReadOnlyList<ReceivedMethodCall> sendMessageCalls = QueueBuilder.JobsQueue?.GetReceivedMethodCalls() ?? [];
        TestResultWithData<TriggerJobWithDataTestResult> result = new()
        {
            TestCaseId = 1,
            Data = new TriggerJobWithDataTestResult
            {
                StatusCode = responseMessage.StatusCode,
                Response = response.PrettifyJson(4),
                JobEntitiesDb = jobEntitiesDb,
                SendMessageCalls = sendMessageCalls,
                LogMessages = _logMessages.GetSerialized(6)
            }
        };

        return result;
    }

    private class TriggerJobWithDataTestResult : HttpTestResult
    {
        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];

        public IReadOnlyList<ReceivedMethodCall> SendMessageCalls { get; init; } = [];
    }
}
