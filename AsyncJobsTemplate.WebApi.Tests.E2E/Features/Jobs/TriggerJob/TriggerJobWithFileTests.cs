using System.Net.Mime;
using AsyncJobsTemplate.Core.Commands.TriggerJob;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.Shared.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Services;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCollections;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Net.Http.Headers;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJob;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class TriggerJobWithFileTests
{
    private readonly string _endpointUrlPath = "/jobs/{categoryName}/file";
    private readonly VerifySettings _verifySettings;

    private readonly WebApplicationFactory<Program> _webApiFactory;

    public TriggerJobWithFileTests(WebApiFactory webApiFactory)
    {
        _verifySettings = webApiFactory.VerifySettings;
        _webApiFactory = webApiFactory.DisableAuth();
    }

    [Fact]
    public async Task TriggerJobWithFile_ShouldTriggerJobProcessing_WhenCorrectData()
    {
        using DbContextScope dbScope = new(_webApiFactory);

        HttpRequestMessage requestMessage = BuildRequestMessageWithDataPayload();
        HttpResponseMessage responseMessage =
            await _webApiFactory.MockJobsQueue().CreateClient().SendAsync(requestMessage);
        TriggerJobResult? response = await responseMessage.GetResponseAsync<TriggerJobResult>();

        IReadOnlyList<JobEntity> jobEntitiesDb = await JobsData.GetJobsAsync(dbScope);
        IReadOnlyList<StorageAccountFile> inputFiles = await StorageAccountFilesData.GetInputFilesAsync(_webApiFactory);
        IReadOnlyList<ReceivedMethodCall> sendMessageCalls = QueueBuilder.JobsQueue?.GetReceivedMethodCalls() ?? [];

        TestResultWithData<TriggerJobWithFileTestResult> result = new()
        {
            TestCaseId = 1,
            Data = new TriggerJobWithFileTestResult
            {
                StatusCode = responseMessage.StatusCode,
                Response = response,
                JobEntitiesDb = jobEntitiesDb,
                InputFilesStorageAccount = inputFiles,
                SendMessageCalls = sendMessageCalls
            }
        };

        await Verify(result, _verifySettings);
    }

    private HttpRequestMessage BuildRequestMessageWithDataPayload()
    {
        string categoryName = nameof(TriggerJobWithFile_ShouldTriggerJobProcessing_WhenCorrectData);
        string endpointPath = BuildEndpointUrlPath(categoryName);

        byte[] file = EmbeddedFile.Get("Features/Jobs/TriggerJob/Assets/test_payload.csv");
        MultipartFormDataContent formData = new();
        ByteArrayContent fileContent = new(file);
        fileContent.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Text.Csv);
        formData.Add(fileContent, "file", "test_payload.csv");

        HttpRequestMessage requestMessage = new(HttpMethod.Post, endpointPath);
        requestMessage.Content = formData;

        return requestMessage;
    }

    private string BuildEndpointUrlPath(string categoryName)
    {
        return _endpointUrlPath.Replace("{categoryName}", categoryName);
    }

    private class TriggerJobWithFileTestResult : TestResponseWithData<TriggerJobResult>
    {
        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];

        public IReadOnlyList<StorageAccountFile> InputFilesStorageAccount { get; init; } = [];

        public IReadOnlyList<ReceivedMethodCall> SendMessageCalls { get; init; } = [];
    }
}
