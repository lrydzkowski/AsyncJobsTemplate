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
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job2.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job2.Data.CorrectTestCases;
using AsyncJobsTemplate.WebApi.Tests.E2E.Features.Jobs.ConsumeJob.Job2.Data.IncorrectTestCases;
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
    public async Task ConsumeJob2_ShouldBeSuccessful()
    {
        List<ConsumeJob2MessageTestResult> results = [];
        foreach (TestCaseData testCase in CorrectTestCasesGenerator.Generate())
        {
            results.Add(await RunAsync(testCase));
        }

        await Verify(results, _verifySettings);
    }

    [Fact]
    public async Task ConsumeJob2_ShouldBeUnsuccessful()
    {
        List<ConsumeJob2MessageTestResult> results = [];
        foreach (TestCaseData testCase in IncorrectTestCasesGenerator.Generate(_azuriteContainer))
        {
            results.Add(await RunAsync(testCase));
        }

        await Verify(results, _verifySettings);
    }

    private async Task<ConsumeJob2MessageTestResult> RunAsync(TestCaseData testCase)
    {
        WebApplicationFactory<Program> webApiFactory = _webApiFactory.WithCustomUserEmail(testCase.UserEmail)
            .WithCustomOptions(testCase.CustomOptions);
        await using TestContextScope contextScope = new(webApiFactory, _logMessages);
        await JobsData.CreateJobsAsync(contextScope, testCase);

        ConsumeContext<JobMessage>? context = Substitute.For<ConsumeContext<JobMessage>>()!;
        context.Message.Returns(new JobMessage { JobId = testCase.JobId });

        JobsConsumer jobsConsumer = contextScope.GetRequiredService<JobsConsumer>();
        await jobsConsumer.Consume(context);

        ConsumeJob2MessageTestResult result = new()
        {
            TestCaseId = testCase.TestCaseId,
            JobEntitiesDb = await JobsData.GetJobsAsync(contextScope),
            OutputFilesStorageAccount =
                testCase.UseAzureStorageAccount ? await FilesData.GetOutputFilesAsync(contextScope) : [],
            LogMessages = _logMessages.GetSerialized(6)
        };

        return result;
    }

    private class ConsumeJob2MessageTestResult : ITestResult
    {
        public IReadOnlyList<JobEntity> JobEntitiesDb { get; init; } = [];
        public IReadOnlyList<StorageAccountFile> OutputFilesStorageAccount { get; init; } = [];
        public int TestCaseId { get; init; }
        public string? LogMessages { get; init; }
    }
}
