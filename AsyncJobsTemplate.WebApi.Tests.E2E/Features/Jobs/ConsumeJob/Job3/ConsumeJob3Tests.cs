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
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job3.Data.CorrectTestCases;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;
using Testcontainers.Azurite;
using WireMock.Server;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job3;

[Collection(MainTestsCollection.CollectionName)]
[Trait(TestConstants.Category, MainTestsCollection.CollectionName)]
public class ConsumeJob3Tests
{
    private readonly AzuriteContainer _azuriteContainer;
    private readonly LogMessages _logMessages;
    private readonly VerifySettings _verifySettings;
    private readonly WebApplicationFactory<Program> _webApiFactory;
    private readonly WireMockServer _wireMockServer;

    public ConsumeJob3Tests(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory.DisableJobsSleep();
        _logMessages = webApiFactory.LogMessages;
        _verifySettings = webApiFactory.VerifySettings;
        _azuriteContainer = webApiFactory.AzuriteContainer;
        _wireMockServer = webApiFactory.WireMockServer;
    }

    [Fact]
    public async Task ConsumeJob3_ShouldBeSuccessful()
    {
        List<ConsumeJob3MessageTestResult> results = [];
        foreach (TestCaseData testCase in CorrectTestCasesGenerator.Generate())
        {
            results.Add(await RunAsync(testCase));
        }

        await Verify(results, _verifySettings);
    }

    private async Task<ConsumeJob3MessageTestResult> RunAsync(TestCaseData testCase)
    {
        WebApplicationFactory<Program> webApiFactory = _webApiFactory.WithDependencies(_wireMockServer, testCase);
        await using TestContextScope contextScope = new(webApiFactory, _logMessages);
        await contextScope.CreateJobsAsync(testCase);

        ConsumeContext<JobMessage>? context = Substitute.For<ConsumeContext<JobMessage>>()!;
        context.Message.Returns(new JobMessage { JobId = testCase.JobId });

        JobsConsumer jobsConsumer = contextScope.GetRequiredService<JobsConsumer>();
        await jobsConsumer.Consume(context);

        ConsumeJob3MessageTestResult result = new()
        {
            TestCaseId = testCase.TestCaseId,
            JobEntitiesDb = await contextScope.GetJobsAsync(),
            OutputFilesStorageAccount = await contextScope.GetOutputFilesAsync(),
            LogMessages = _logMessages.GetSerialized(6)
        };

        return result;
    }

    private class ConsumeJob3MessageTestResult : ITestResult
    {
        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];
        public IReadOnlyList<StorageAccountFile> OutputFilesStorageAccount { get; init; } = [];
        public int TestCaseId { get; init; }
        public string? LogMessages { get; init; }
    }
}
