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
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job1.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job1.Data.CorrectTestCases;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job1.Data.IncorrectTestCases;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.MsSql;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job1;

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
    public async Task ConsumeJob1_ShouldBeSuccessful()
    {
        List<ConsumeJob1MessageTestResult> results = [];
        foreach (TestCaseData testCase in CorrectTestCasesGenerator.Generate())
        {
            results.Add(await RunAsync(testCase));
        }

        await Verify(results, _verifySettings);
    }

    [Fact]
    public async Task ConsumeJob1_ShouldBeUnsuccessful()
    {
        List<ConsumeJob1MessageTestResult> results = [];
        foreach (TestCaseData testCase in IncorrectTestCasesGenerator.Generate(_dbContainer))
        {
            results.Add(await RunAsync(testCase));
        }

        await Verify(results, _verifySettings);
    }

    private async Task<ConsumeJob1MessageTestResult> RunAsync(TestCaseData testCase)
    {
        WebApplicationFactory<Program> webApiFactory = _webApiFactory.WithCustomUserEmail(testCase.UserEmail)
            .WithCustomOptions(testCase.CustomOptions);
        await using TestContextScope contextScope = new(webApiFactory, _logMessages);
        if (testCase.UseDatabase)
        {
            await contextScope.CreateJobsAsync(testCase);
        }

        JobMessage jobMessage = new() { JobId = testCase.JobId };
        await contextScope.GetRequiredService<JobsQueueConsumer>().ConsumeMessageAsync(jobMessage);

        ConsumeJob1MessageTestResult result = new()
        {
            TestCaseId = testCase.TestCaseId,
            JobId = testCase.JobId,
            JobEntitiesDb = testCase.UseDatabase ? await contextScope.GetJobsAsync() : [],
            LogMessages = _logMessages.GetSerialized(6)
        };

        return result;
    }

    private class ConsumeJob1MessageTestResult : ITestResult
    {
        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];
        public Guid JobId { get; init; }
        public int TestCaseId { get; init; }
        public string? LogMessages { get; init; }
    }
}
