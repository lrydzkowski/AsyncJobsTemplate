using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.Shared.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.Db;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCollections;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJob.Data;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.GetJob;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class GetJobTests
{
    private readonly string _endpointUrlPath = "/jobs/{jobId}";
    private readonly LogMessages _logMessages;
    private readonly VerifySettings _verifySettings;
    private readonly WebApplicationFactory<Program> _webApiFactory;

    public GetJobTests(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory.DisableAuth();
        _logMessages = webApiFactory.LogMessages;
        _verifySettings = webApiFactory.VerifySettings;
    }

    [Fact]
    public async Task GetJob_ShouldReturnCorrectAnswer()
    {
        List<TestResultWithData<GetJobTestResult>>? results = [];

        foreach (TestCaseData testCaseData in GetJobTestCases.Get())
        {
            await using TestContextScope contextScope = new(_webApiFactory, _logMessages);

            HttpClient client = _webApiFactory.CreateClient();
            (HttpResponseMessage responseMessage, string response) = await SendRequestAsync(client, testCaseData.JobId);
            TestResultWithData<GetJobTestResult> result = await BuildTestResultAsync(
                testCaseData,
                contextScope,
                responseMessage,
                response
            );

            results.Add(result);
        }

        await Verify(results, _verifySettings);
    }

    private async Task<(HttpResponseMessage responseMessage, string response)> SendRequestAsync(
        HttpClient client,
        string jobId
    )
    {
        HttpRequestMessage requestMessage = new(HttpMethod.Get, _endpointUrlPath.Replace("{jobId}", jobId));
        HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
        string response = await responseMessage.GetResponseMessageAsync();

        return (responseMessage, response);
    }

    private async Task<TestResultWithData<GetJobTestResult>> BuildTestResultAsync(
        TestCaseData testCaseData,
        TestContextScope contextScope,
        HttpResponseMessage responseMessage,
        string response
    )
    {
        IReadOnlyList<JobEntity> jobEntitiesDb = await JobsData.GetJobsAsync(contextScope);
        TestResultWithData<GetJobTestResult> result = new()
        {
            TestCaseId = testCaseData.TestCaseId,
            Data = new GetJobTestResult
            {
                JobId = testCaseData.JobId,
                StatusCode = responseMessage.StatusCode,
                Response = response.PrettifyJson(6),
                JobEntitiesDb = jobEntitiesDb,
                LogMessages = _logMessages.GetSerialized(6)
            }
        };

        return result;
    }

    private class GetJobTestResult : HttpTestResult
    {
        public string JobId { get; init; } = "";

        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];
    }
}
