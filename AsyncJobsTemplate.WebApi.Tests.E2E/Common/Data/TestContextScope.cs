using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Logging;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using DbContextScope = AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.Db.ContextScope;
using StorageAccountContextScope = AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.StorageAccount.ContextScope;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;

internal class TestContextScope : IAsyncDisposable
{
    public TestContextScope(WebApplicationFactory<Program> webApiFactory, LogMessages logMessages)
    {
        LogMessages = logMessages;
        ServiceScope = webApiFactory.Services.CreateScope();
        Db = new DbContextScope(ServiceScope.ServiceProvider);
        StorageAccount = new StorageAccountContextScope(ServiceScope.ServiceProvider);
        JobsQueue = Substitute.For<IJobsQueue>();
    }

    public LogMessages LogMessages { get; }

    public IServiceScope ServiceScope { get; }

    public DbContextScope Db { get; }

    public StorageAccountContextScope StorageAccount { get; }

    public IJobsQueue JobsQueue { get; set; }

    public async ValueTask DisposeAsync()
    {
        await StorageAccount.DisposeAsync();
        Db.Dispose();
        ServiceScope.Dispose();
        LogMessages.Clear();
    }

    public TService GetRequiredService<TService>() where TService : notnull
    {
        return ServiceScope.ServiceProvider.GetRequiredService<TService>();
    }
}
