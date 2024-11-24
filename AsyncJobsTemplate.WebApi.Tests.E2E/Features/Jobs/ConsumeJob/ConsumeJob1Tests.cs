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

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class ConsumeJob1Tests
{
    private readonly LogMessages _logMessages;
    private readonly VerifySettings _verifySettings;
    private readonly WebApplicationFactory<Program> _webApiFactory;

    public ConsumeJob1Tests(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory.DisableJobsSleep();
        _logMessages = webApiFactory.LogMessages;
        _verifySettings = webApiFactory.VerifySettings;
    }

    [Fact]
    public async Task ConsumeJob1Message_ShouldBeSuccessful_WhenCorrectData()
    {
        Guid jobIb = Guid.NewGuid();
        string categoryName = Job1Handler.Name;

        await using TestContextScope contextScope = new(_webApiFactory, _logMessages);
        await JobsData.CreateJobAsync(contextScope, jobIb, categoryName);

        await RunTestAsync(contextScope, jobIb);
        TestResultWithData<ConsumeJobMessageTestResult> result = await BuildTestResultAsync(contextScope);

        await Verify(result, _verifySettings);
    }

    private static async Task RunTestAsync(TestContextScope contextScope, Guid jobIb)
    {
        ConsumeContext<JobMessage>? context = Substitute.For<ConsumeContext<JobMessage>>()!;
        context.Message.Returns(new JobMessage { JobId = jobIb });

        JobsConsumer jobsConsumer = contextScope.GetRequiredService<JobsConsumer>();
        await jobsConsumer.Consume(context);
    }

    private async Task<TestResultWithData<ConsumeJobMessageTestResult>> BuildTestResultAsync(
        TestContextScope contextScope
    )
    {
        IReadOnlyList<JobEntity> jobEntitiesDb = await JobsData.GetJobsAsync(contextScope);
        TestResultWithData<ConsumeJobMessageTestResult> result = new()
        {
            TestCaseId = 1,
            Data = new ConsumeJobMessageTestResult
            {
                JobEntitiesDb = jobEntitiesDb,
                LogMessages = _logMessages.GetSerialized(6)
            }
        };

        return result;
    }

    private class ConsumeJobMessageTestResult
    {
        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];

        public string LogMessages { get; init; } = "";
    }
}
