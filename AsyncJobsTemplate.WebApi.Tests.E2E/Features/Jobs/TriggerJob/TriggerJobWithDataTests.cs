using AsyncJobsTemplate.Core.Commands.TriggerJob;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.Shared.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCollections;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJob;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class TriggerJobWithDataTests
{
    private readonly string _endpointUrlPath = "/jobs/{categoryName}";
    private readonly VerifySettings _verifySettings;

    private readonly WebApplicationFactory<Program> _webApiFactory;

    public TriggerJobWithDataTests(WebApiFactory webApiFactory)
    {
        _verifySettings = webApiFactory.VerifySettings;
        _webApiFactory = webApiFactory.DisableAuth();
    }

    [Fact]
    public async Task TriggerJobWithData_ShouldTriggerJobProcessing_WhenCorrectData()
    {
        using DbContextScope dbScope = new(_webApiFactory);

        HttpRequestMessage requestMessage = BuildRequestMessageWithDataPayload();
        HttpResponseMessage responseMessage =
            await _webApiFactory.MockJobsQueue().CreateClient().SendAsync(requestMessage);
        TriggerJobResult? response = await responseMessage.GetResponseAsync<TriggerJobResult>();

        IReadOnlyList<JobEntity> jobEntitiesDb = await JobsData.GetJobsAsync(dbScope);
        IReadOnlyList<ReceivedMethodCall> sendMessageCalls = QueueBuilder.JobsQueue?.GetReceivedMethodCalls() ?? [];

        RequestResultWithData<TriggerJobWithDataTestResult> result = new()
        {
            TestCaseId = 1,
            StatusCode = responseMessage.StatusCode,
            Data = new TriggerJobWithDataTestResult
            {
                Response = response,
                JobEntitiesDb = jobEntitiesDb,
                SendMessageCalls = sendMessageCalls
            }
        };

        await Verify(result, _verifySettings);
    }

    private HttpRequestMessage BuildRequestMessageWithDataPayload()
    {
        string categoryName = nameof(TriggerJobWithData_ShouldTriggerJobProcessing_WhenCorrectData);
        string endpointPath = BuildEndpointUrlPath(categoryName);
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

    private string BuildEndpointUrlPath(string categoryName)
    {
        return _endpointUrlPath.Replace("{categoryName}", categoryName);
    }

    private class TriggerJobWithDataTestResult
    {
        public TriggerJobResult? Response { get; init; }

        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];

        public IReadOnlyList<ReceivedMethodCall> SendMessageCalls { get; init; } = [];
    }
}