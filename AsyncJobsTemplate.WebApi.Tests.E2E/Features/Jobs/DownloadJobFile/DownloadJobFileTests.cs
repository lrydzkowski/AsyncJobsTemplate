using System.Net;
using AsyncJobsTemplate.Shared.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCollections;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.DownloadJobFile.Data;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.DownloadJobFile;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class DownloadJobFileTests
{
    private readonly string _endpointUrlPath = "/jobs/{jobId}/file";
    private readonly LogMessages _logMessages;
    private readonly VerifySettings _verifySettings;
    private readonly WebApplicationFactory<Program> _webApiFactory;

    public DownloadJobFileTests(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory.DisableAuth();
        _logMessages = webApiFactory.LogMessages;
        _verifySettings = webApiFactory.VerifySettings;
    }

    [Fact]
    public async Task DownloadJobFile_ShouldReturnExpectedResponse()
    {
        List<TestResultWithData<DownloadJobFileTestResult>>? results = [];
        foreach (TestCaseData testCaseData in TestCasesGenerator.Get())
        {
            await using TestContextScope contextScope = new(_webApiFactory, _logMessages);

            HttpClient client = (await _webApiFactory.BuildAsync(contextScope, testCaseData)).CreateClient();
            (HttpStatusCode responseStatusCode, string response) = await SendRequestAsync(client, testCaseData);
            TestResultWithData<DownloadJobFileTestResult> result = BuildTestResult(
                testCaseData,
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
        using HttpRequestMessage requestMessage = new(
            HttpMethod.Get,
            _endpointUrlPath.Replace("{jobId}", testCaseData.JobId)
        );
        using HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
        HttpStatusCode responseStatusCode = responseMessage.StatusCode;
        string response = await responseMessage.GetResponseMessageAsync();

        return (responseStatusCode, response);
    }

    private TestResultWithData<DownloadJobFileTestResult> BuildTestResult(
        TestCaseData testCaseData,
        HttpStatusCode responseStatusCode,
        string response
    )
    {
        TestResultWithData<DownloadJobFileTestResult> result = new()
        {
            TestCaseId = testCaseData.TestCaseId,
            Data = new DownloadJobFileTestResult
            {
                JobId = testCaseData.JobId,
                StatusCode = responseStatusCode,
                Response = response.PrettifyJson(6),
                LogMessages = _logMessages.GetSerialized(6)
            }
        };

        return result;
    }

    private class DownloadJobFileTestResult : HttpTestResult
    {
        public string JobId { get; init; } = "";
    }
}
