using System.Net;
using System.Net.Mime;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.Shared.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.Db;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.StorageAccount;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Services;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCollections;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Net.Http.Headers;
using Testcontainers.Azurite;
using Testcontainers.MsSql;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJob;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class TriggerJobWithFileTests
{
    private readonly AzuriteContainer _azuriteContainer;
    private readonly MsSqlContainer _dbContainer;
    private readonly string _endpointUrlPath = "/jobs/{categoryName}/file";
    private readonly LogMessages _logMessages;
    private readonly VerifySettings _verifySettings;
    private readonly WebApplicationFactory<Program> _webApiFactory;

    public TriggerJobWithFileTests(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory.DisableAuth();
        _logMessages = webApiFactory.LogMessages;
        _verifySettings = webApiFactory.VerifySettings;
        _dbContainer = webApiFactory.DbContainer;
        _azuriteContainer = webApiFactory.AzuriteContainer;
    }

    [Fact]
    public async Task TriggerJobWithFile_ShouldTriggerJobProcessing_WhenCorrectData()
    {
        await using TestContextScope contextScope = new(_webApiFactory, _logMessages);

        HttpClient client = _webApiFactory.MockJobsQueue(contextScope.JobsQueue).CreateClient();
        (HttpStatusCode responseStatusCode, string response) = await SendRequestAsync(client);
        TestResultWithData<TriggerJobWithFileTestResult> result = await BuildTestResultAsync(
            contextScope,
            responseStatusCode,
            response
        );

        await Verify(result, _verifySettings);
    }

    [Fact]
    public async Task TriggerJobWithFile_ShouldReturnError_WhenNoAccessToDatabase()
    {
        await using TestContextScope contextScope = new(_webApiFactory, _logMessages);

        HttpClient client = _webApiFactory.MakeDbConnectionStringIncorrect(_dbContainer.GetConnectionString())
            .MockJobsQueue(contextScope.JobsQueue)
            .CreateClient();
        (HttpStatusCode responseStatusCode, string response) = await SendRequestAsync(client);
        TestResultWithData<TriggerJobWithFileTestResult> result = await BuildTestResultAsync(
            contextScope,
            responseStatusCode,
            response
        );

        await Verify(result, _verifySettings);
    }

    [Fact]
    public async Task TriggerJobWithFile_ShouldReturnError_WhenSavingFileNotWork()
    {
        await using TestContextScope contextScope = new(_webApiFactory, _logMessages);

        HttpClient client = _webApiFactory
            .MakeStorageAccountConnectionStringIncorrect(_azuriteContainer.GetConnectionString())
            .MockJobsQueue(contextScope.JobsQueue)
            .CreateClient();
        (HttpStatusCode responseStatusCode, string response) = await SendRequestAsync(client);
        TestResultWithData<TriggerJobWithFileTestResult> result = await BuildTestResultAsync(
            contextScope,
            responseStatusCode,
            response
        );

        await Verify(result, _verifySettings);
    }

    private async Task<(HttpStatusCode responseStatusCode, string response)> SendRequestAsync(HttpClient client)
    {
        using HttpRequestMessage requestMessage = BuildRequestMessage();
        using HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
        HttpStatusCode responseStatusCode = responseMessage.StatusCode;
        string response = await responseMessage.GetResponseMessageAsync();

        return (responseStatusCode, response);
    }

    private HttpRequestMessage BuildRequestMessage()
    {
        string categoryName = nameof(TriggerJobWithFile_ShouldTriggerJobProcessing_WhenCorrectData);
        string endpointPath = _endpointUrlPath.Replace("{categoryName}", categoryName);

        byte[] file = EmbeddedFile.Get("Features/Jobs/TriggerJob/Assets/test_payload.csv");
        MultipartFormDataContent formData = new();
        ByteArrayContent fileContent = new(file);
        fileContent.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Text.Csv);
        formData.Add(fileContent, "file", "test_payload.csv");

        HttpRequestMessage requestMessage = new(HttpMethod.Post, endpointPath);
        requestMessage.Content = formData;

        return requestMessage;
    }

    private async Task<TestResultWithData<TriggerJobWithFileTestResult>> BuildTestResultAsync(
        TestContextScope contextScope,
        HttpStatusCode responseStatusCode,
        string response
    )
    {
        IReadOnlyList<JobEntity> jobEntitiesDb = await JobsData.GetJobsAsync(contextScope);
        IReadOnlyList<StorageAccountFile> inputFiles = await FilesData.GetInputFilesAsync(_webApiFactory);
        IReadOnlyList<ReceivedMethodCall> sendMessageCalls = contextScope.JobsQueue.GetReceivedMethodCalls() ?? [];
        TestResultWithData<TriggerJobWithFileTestResult> result = new()
        {
            TestCaseId = 1,
            Data = new TriggerJobWithFileTestResult
            {
                StatusCode = responseStatusCode,
                Response = response.PrettifyJson(4),
                JobEntitiesDb = jobEntitiesDb,
                InputFilesStorageAccount = inputFiles,
                SendMessageCalls = sendMessageCalls,
                LogMessages = _logMessages.GetSerialized(6)
            }
        };

        return result;
    }

    private class TriggerJobWithFileTestResult : HttpTestResult
    {
        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];

        public IReadOnlyList<StorageAccountFile> InputFilesStorageAccount { get; init; } = [];

        public IReadOnlyList<ReceivedMethodCall> SendMessageCalls { get; init; } = [];
    }
}
