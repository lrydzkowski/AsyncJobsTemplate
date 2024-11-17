using AsyncJobsTemplate.Infrastructure.Db;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;

internal class DbContextScope : IDisposable
{
    private readonly IServiceScope _scope;

    public DbContextScope(WebApplicationFactory<Program> webApiFactory)
    {
        _scope = webApiFactory.Server.Services.CreateScope();
    }

    public void Dispose()
    {
        AppDbContext context = GetDbContext();

        RemoveData(JobEntity.TableName, context);

        _scope.Dispose();
    }

    public AppDbContext GetDbContext()
    {
        return _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    private void RemoveData(string tableName, AppDbContext context)
    {
        string sql = $"""
                          IF EXISTS (SELECT 1 FROM [{tableName}])
                          BEGIN
                              DELETE FROM [{tableName}];
                              DBCC CHECKIDENT('[{tableName}]', RESEED, 0);
                          END
                      """;
        context.Database.ExecuteSqlRaw(sql);
    }
}
