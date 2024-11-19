using AsyncJobsTemplate.Core.Jobs;
using AsyncJobsTemplate.Infrastructure.Azure.ServiceBus;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.WebApi.Consumers;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCollections;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class ConsumeJob1Tests
{
    private readonly VerifySettings _verifySettings;

    private readonly WebApplicationFactory<Program> _webApiFactory;

    public ConsumeJob1Tests(WebApiFactory webApiFactory)
    {
        _verifySettings = webApiFactory.VerifySettings;
        _webApiFactory = webApiFactory.DisableJobsSleep();
    }

    [Fact]
    public async Task ConsumeJob1Message_ShouldBeSuccessful_WhenCorrectData()
    {
        Guid jobIb = Guid.NewGuid();
        string categoryName = Job1Handler.Name;

        using IServiceScope serviceScope = _webApiFactory.Services.CreateScope();
        using DbContextScope dbScope = new(serviceScope.ServiceProvider);
        await using StorageAccountContextScope storageAccountScope = new(serviceScope.ServiceProvider);
        await DbJobsData.CreateJobAsync(dbScope, jobIb, categoryName);

        ConsumeContext<JobMessage>? context = Substitute.For<ConsumeContext<JobMessage>>()!;
        context.Message.Returns(new JobMessage { JobId = jobIb });

        JobsConsumer jobsConsumer = serviceScope.ServiceProvider.GetRequiredService<JobsConsumer>();
        await jobsConsumer.Consume(context);

        IReadOnlyList<JobEntity> jobEntitiesDb = await DbJobsData.GetJobsAsync(dbScope);

        TestResultWithData<ConsumeJobMessageTestResult> result = new()
        {
            TestCaseId = 1,
            Data = new ConsumeJobMessageTestResult
            {
                JobEntitiesDb = jobEntitiesDb
            }
        };

        await Verify(result, _verifySettings);
    }

    private class ConsumeJobMessageTestResult
    {
        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];
    }
}
