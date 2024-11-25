using AsyncJobsTemplate.Core.Jobs;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.WebApi.Consumers;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.Db;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCollections;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;
using Testcontainers.MsSql;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class ConsumeJob1Tests
{
    private readonly MsSqlContainer _dbContainer;
    private readonly LogMessages _logMessages;
    private readonly VerifySettings _verifySettings;
    private readonly WebApplicationFactory<Program> _webApiFactory;

    public ConsumeJob1Tests(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory.DisableJobsSleep();
        _logMessages = webApiFactory.LogMessages;
        _verifySettings = webApiFactory.VerifySettings;
        _dbContainer = webApiFactory.DbContainer;
    }

    [Fact]
    public async Task ConsumeJob1Message_ShouldBeSuccessful_WhenCorrectData()
    {
        Guid jobId = Guid.NewGuid();
        string categoryName = Job1Handler.Name;

        await using TestContextScope contextScope = new(_webApiFactory, _logMessages);
        await JobsData.CreateJobAsync(contextScope, jobId, categoryName);

        await RunTestAsync(contextScope, jobId);
        TestResultWithData<ConsumeJob1MessageTestResult> result = await BuildTestResultAsync(contextScope);

        await Verify(result, _verifySettings);
    }

    [Fact]
    public async Task ConsumeJob1Message_ShouldBeUnsuccessful_WhenNoAccessToDatabase()
    {
        Guid jobId = Guid.NewGuid();

        WebApplicationFactory<Program> webApiFactory =
            _webApiFactory.MakeDbConnectionStringIncorrect(_dbContainer.GetConnectionString());
        await using TestContextScope contextScope = new(webApiFactory, _logMessages);

        await RunTestAsync(contextScope, jobId);
        TestResultWithData<ConsumeJob1MessageTestResult> result = await BuildTestResultAsync(contextScope, false);

        await Verify(result, _verifySettings);
    }

    [Fact]
    public async Task ConsumeJob1Message_ShouldBeUnsuccessful_WhenJobNotExist()
    {
        Guid existingJobId = Guid.NewGuid();
        Guid nonExistingJobId = Guid.NewGuid();
        string categoryName = Job1Handler.Name;

        await using TestContextScope contextScope = new(_webApiFactory, _logMessages);
        await JobsData.CreateJobAsync(contextScope, existingJobId, categoryName);

        await RunTestAsync(contextScope, nonExistingJobId);
        TestResultWithData<ConsumeJob1MessageTestResult> result = await BuildTestResultAsync(contextScope);

        await Verify(result, _verifySettings);
    }

    [Fact]
    public async Task ConsumeJob1Message_ShouldBeUnsuccessful_WhenJobNotRegistered()
    {
        Guid jobId = Guid.NewGuid();
        string incorrectCategoryName = Job1Handler.Name + "-Incorrect";

        await using TestContextScope contextScope = new(_webApiFactory, _logMessages);
        await JobsData.CreateJobAsync(contextScope, jobId, incorrectCategoryName);

        await RunTestAsync(contextScope, jobId);
        TestResultWithData<ConsumeJob1MessageTestResult> result = await BuildTestResultAsync(contextScope);

        await Verify(result, _verifySettings);
    }

    private static async Task RunTestAsync(TestContextScope contextScope, Guid jobId)
    {
        ConsumeContext<JobMessage>? context = Substitute.For<ConsumeContext<JobMessage>>()!;
        context.Message.Returns(new JobMessage { JobId = jobId });

        JobsConsumer jobsConsumer = contextScope.GetRequiredService<JobsConsumer>();
        await jobsConsumer.Consume(context);
    }

    private async Task<TestResultWithData<ConsumeJob1MessageTestResult>> BuildTestResultAsync(
        TestContextScope contextScope,
        bool withDbData = true
    )
    {
        IReadOnlyList<JobEntity> jobEntitiesDb = withDbData ? await JobsData.GetJobsAsync(contextScope) : [];
        TestResultWithData<ConsumeJob1MessageTestResult> result = new()
        {
            TestCaseId = 1,
            Data = new ConsumeJob1MessageTestResult
            {
                JobEntitiesDb = jobEntitiesDb,
                LogMessages = _logMessages.GetSerialized(6)
            }
        };

        return result;
    }

    private class ConsumeJob1MessageTestResult
    {
        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];

        public string LogMessages { get; init; } = "";
    }
}
