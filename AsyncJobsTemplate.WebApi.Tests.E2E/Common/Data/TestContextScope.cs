using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using DbContextScope = AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.Db.ContextScope;
using StorageAccountContextScope = AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.StorageAccount.ContextScope;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;

internal class TestContextScope : IAsyncDisposable
{
    public TestContextScope(WebApplicationFactory<Program> webApiFactory)
    {
        ServiceScope = webApiFactory.Services.CreateScope();
        Db = new DbContextScope(ServiceScope.ServiceProvider);
        StorageAccount = new StorageAccountContextScope(ServiceScope.ServiceProvider);
    }

    public IServiceScope ServiceScope { get; }

    public DbContextScope Db { get; }

    public StorageAccountContextScope StorageAccount { get; }

    public async ValueTask DisposeAsync()
    {
        await StorageAccount.DisposeAsync();
        Db.Dispose();
        ServiceScope.Dispose();
    }

    public TService GetRequiredService<TService>() where TService : notnull
    {
        return ServiceScope.ServiceProvider.GetRequiredService<TService>();
    }
}
