using System.Net;
using System.Net.Mime;
using System.Reflection;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.Shared.Extensions;
using AsyncJobsTemplate.Shared.Services;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.Db;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.StorageAccount;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Extensions;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCollections;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithFile.Data.CorrectTestCases;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithFile.Data.IncorrectTestCases;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Net.Http.Headers;
using Testcontainers.Azurite;
using Testcontainers.MsSql;
using TestCaseData = AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithFile.Data.TestCaseData;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.TriggerJobWithFile;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class TriggerJobWithFileTests
{
    private const string CategoryNamePlaceholder = "{categoryName}";
    private readonly AzuriteContainer _azuriteContainer;
    private readonly MsSqlContainer _dbContainer;
    private readonly string _endpointUrlPath = $"/jobs/{CategoryNamePlaceholder}/file";
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
    public async Task TriggerJobWithFile_ShouldReturnCorrectResponse()
    {
        List<TriggerJobWithFileTestResult> results = [];
        foreach (TestCaseData testCase in CorrectTestCasesGenerator.Generate())
        {
            results.Add(await RunAsync(testCase));
        }

        await Verify(results, _verifySettings);
    }

    [Fact]
    public async Task TriggerJobWithFile_ShouldReturnIncorrectResponse()
    {
        List<TriggerJobWithFileTestResult> results = [];
        foreach (TestCaseData testCase in IncorrectTestCasesGenerator.Generate(_dbContainer, _azuriteContainer))
        {
            results.Add(await RunAsync(testCase));
        }

        await Verify(results, _verifySettings);
    }

    private async Task<TriggerJobWithFileTestResult> RunAsync(TestCaseData testCase)
    {
        WebApplicationFactory<Program> webApiFactory = _webApiFactory.WithCustomUserEmail(testCase.UserEmail);
        await using TestContextScope contextScope = new(webApiFactory, _logMessages);

        HttpClient client = webApiFactory
            .WithCustomOptions(testCase.CustomOptions)
            .MockJobsQueue(contextScope.JobsQueue)
            .CreateClient();
        using HttpRequestMessage requestMessage = BuildRequestMessage(testCase);
        using HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
        string response = await responseMessage.GetResponseMessageAsync();

        TriggerJobWithFileTestResult result = new()
        {
            JobEntitiesDb = await JobsData.GetJobsAsync(contextScope),
            InputFilesStorageAccount = await FilesData.GetInputFilesAsync(contextScope),
            SendMessageCalls = contextScope.JobsQueue.GetReceivedMethodCalls() ?? [],
            TestCaseId = testCase.TestCaseId,
            StatusCode = responseMessage.StatusCode,
            Response = response.PrettifyJson(4),
            LogMessages = _logMessages.GetSerialized(6)
        };

        return result;
    }

    private HttpRequestMessage BuildRequestMessage(TestCaseData testCAse)
    {
        HttpRequestMessage? requestMessage = new(HttpMethod.Post, BuildUrl(testCAse));
        byte[] file = EmbeddedFile.Get(
            $"Features/Jobs/TriggerJobWithFile/Data/Assets/{testCAse.FileToSend}",
            Assembly.GetExecutingAssembly()
        );
        MultipartFormDataContent formData = new();
        ByteArrayContent fileContent = new(file);
        fileContent.Headers.Add(HeaderNames.ContentType, MediaTypeNames.Text.Csv);
        formData.Add(fileContent, "file", testCAse.FileToSend);
        requestMessage.Content = formData;

        return requestMessage;
    }

    private string BuildUrl(TestCaseData testCase)
    {
        return _endpointUrlPath.Replace(CategoryNamePlaceholder, testCase.CategoryName);
    }

    private class TriggerJobWithFileTestResult : IHttpTestResult
    {
        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];
        public IReadOnlyList<StorageAccountFile> InputFilesStorageAccount { get; init; } = [];
        public IReadOnlyList<ReceivedMethodCall> SendMessageCalls { get; init; } = [];
        public int TestCaseId { get; init; }
        public HttpStatusCode StatusCode { get; init; }
        public string? Response { get; init; }
        public string? LogMessages { get; init; }
    }
}
