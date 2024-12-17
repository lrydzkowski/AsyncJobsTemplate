using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.WebApi.Consumers;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.Db;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.StorageAccount;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCollections;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job3.Data;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;
using WireMock.Server;
using CorrectTestCasesGenerator =
    AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job3.Data.CorrectTestCases.TestCasesGenerator;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job3;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class ConsumeJob3Tests
{
    private readonly LogMessages _logMessages;
    private readonly VerifySettings _verifySettings;
    private readonly WebApplicationFactory<Program> _webApiFactory;
    private readonly WireMockServer _wireMockServer;

    public ConsumeJob3Tests(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory;
        _logMessages = webApiFactory.LogMessages;
        _verifySettings = webApiFactory.VerifySettings;
        _wireMockServer = webApiFactory.WireMockServer;
    }

    [Fact]
    public async Task ConsumeJob3Message_ShouldBeSuccessful_WhenCorrectData()
    {
        List<TestResultWithData<ConsumeJob3MessageTestResult>> results = [];
        WebApplicationFactory<Program> webApiFactory = _webApiFactory;
        foreach (TestCaseData testCaseData in CorrectTestCasesGenerator.Get())
        {
            webApiFactory = MockData(testCaseData, webApiFactory);
            await using TestContextScope contextScope = new(webApiFactory, _logMessages);
            await JobsData.CreateJobAsync(contextScope, testCaseData.JobId, testCaseData.CategoryName, new object());

            await RunTestAsync(contextScope, testCaseData.JobId);
            results.Add(await BuildTestResultAsync(contextScope));
        }

        await Verify(results, _verifySettings);
    }

    private WebApplicationFactory<Program> MockData(
        TestCaseData testCaseData,
        WebApplicationFactory<Program> webApiFactory
    )
    {
        webApiFactory = webApiFactory.MockGetTodo(_wireMockServer, testCaseData);

        return webApiFactory;
    }

    private static async Task RunTestAsync(TestContextScope contextScope, Guid jobId)
    {
        ConsumeContext<JobMessage>? context = Substitute.For<ConsumeContext<JobMessage>>()!;
        context.Message.Returns(new JobMessage { JobId = jobId });

        JobsConsumer jobsConsumer = contextScope.GetRequiredService<JobsConsumer>();
        await jobsConsumer.Consume(context);
    }

    private async Task<TestResultWithData<ConsumeJob3MessageTestResult>> BuildTestResultAsync(
        TestContextScope contextScope
    )
    {
        IReadOnlyList<JobEntity> jobEntitiesDb = await JobsData.GetJobsAsync(contextScope);
        IReadOnlyList<StorageAccountFile> outputFiles = await FilesData.GetOutputFilesAsync(_webApiFactory);
        TestResultWithData<ConsumeJob3MessageTestResult> result = new()
        {
            TestCaseId = 1,
            Data = new ConsumeJob3MessageTestResult
            {
                JobEntitiesDb = jobEntitiesDb,
                OutputFilesStorageAccount = outputFiles,
                LogMessages = _logMessages.GetSerialized(6)
            }
        };

        return result;
    }

    private class ConsumeJob3MessageTestResult
    {
        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];

        public IReadOnlyList<StorageAccountFile> OutputFilesStorageAccount { get; init; } = [];

        public string LogMessages { get; init; } = "";
    }
}
