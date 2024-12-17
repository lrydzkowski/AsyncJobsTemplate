using AsyncJobsTemplate.Core.Jobs.Job2;
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
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;
using Testcontainers.Azurite;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job2;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class ConsumeJob2Tests
{
    private readonly AzuriteContainer _azuriteContainer;
    private readonly LogMessages _logMessages;
    private readonly VerifySettings _verifySettings;
    private readonly WebApplicationFactory<Program> _webApiFactory;

    public ConsumeJob2Tests(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory.DisableJobsSleep();
        _logMessages = webApiFactory.LogMessages;
        _verifySettings = webApiFactory.VerifySettings;
        _azuriteContainer = webApiFactory.AzuriteContainer;
    }

    [Fact]
    public async Task ConsumeJob2Message_ShouldBeSuccessful_WhenCorrectData()
    {
        Guid jobId = Guid.NewGuid();
        string categoryName = Job2Handler.Name;

        await using TestContextScope contextScope = new(_webApiFactory, _logMessages);
        await JobsData.CreateJobAsync(contextScope, jobId, categoryName);

        await RunTestAsync(contextScope, jobId);
        TestResultWithData<ConsumeJob2MessageTestResult> result = await BuildTestResultAsync(contextScope);

        await Verify(result, _verifySettings);
    }

    [Fact]
    public async Task ConsumeJob2Message_ShouldBeUnsuccessful_WhenSavingOutputFileNotWork()
    {
        Guid jobId = Guid.NewGuid();
        string categoryName = Job2Handler.Name;

        WebApplicationFactory<Program> webApiFactory =
            _webApiFactory.MakeStorageAccountConnectionStringIncorrect(_azuriteContainer.GetConnectionString());
        await using TestContextScope contextScope = new(webApiFactory, _logMessages);
        await JobsData.CreateJobAsync(contextScope, jobId, categoryName);

        await RunTestAsync(contextScope, jobId);
        TestResultWithData<ConsumeJob2MessageTestResult> result = await BuildTestResultAsync(contextScope);

        await Verify(result, _verifySettings);
    }

    private static async Task RunTestAsync(TestContextScope contextScope, Guid jobId)
    {
        ConsumeContext<JobMessage>? context = Substitute.For<ConsumeContext<JobMessage>>()!;
        context.Message.Returns(new JobMessage { JobId = jobId });

        JobsConsumer jobsConsumer = contextScope.GetRequiredService<JobsConsumer>();
        await jobsConsumer.Consume(context);
    }

    private async Task<TestResultWithData<ConsumeJob2MessageTestResult>> BuildTestResultAsync(
        TestContextScope contextScope
    )
    {
        IReadOnlyList<JobEntity> jobEntitiesDb = await JobsData.GetJobsAsync(contextScope);
        IReadOnlyList<StorageAccountFile> outputFiles = await FilesData.GetOutputFilesAsync(_webApiFactory);
        TestResultWithData<ConsumeJob2MessageTestResult> result = new()
        {
            TestCaseId = 1,
            Data = new ConsumeJob2MessageTestResult
            {
                JobEntitiesDb = jobEntitiesDb,
                OutputFilesStorageAccount = outputFiles,
                LogMessages = _logMessages.GetSerialized(6)
            }
        };

        return result;
    }

    private class ConsumeJob2MessageTestResult
    {
        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];

        public IReadOnlyList<StorageAccountFile> OutputFilesStorageAccount { get; init; } = [];

        public string LogMessages { get; init; } = "";
    }
}
