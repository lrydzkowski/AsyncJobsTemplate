using AsyncJobsTemplate.Core.Jobs.Job3;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi.Dtos;
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
using WireMock.Server;

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
        Guid jobId = Guid.NewGuid();
        string categoryName = Job3Handler.Name;
        GetTodoResponseDto getTodoResponseDto = new()
        {
            Id = 1,
            UserId = 2,
            Title = "title2",
            Completed = true
        };

        WebApplicationFactory<Program> webApiFactory = _webApiFactory.MockGetTodo(
            _wireMockServer,
            1,
            getTodoResponseDto
        );
        await using TestContextScope contextScope = new(webApiFactory, _logMessages);
        await JobsData.CreateJobAsync(contextScope, jobId, categoryName, new object());

        await RunTestAsync(contextScope, jobId);
        TestResultWithData<ConsumeJob3MessageTestResult> result = await BuildTestResultAsync(contextScope);

        await Verify(result, _verifySettings);
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
