using AsyncJobsTemplate.Infrastructure.Db;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;

internal class DbContextScope : IDisposable
{
    public DbContextScope(IServiceProvider serviceProvider)
    {
        Context = serviceProvider.GetRequiredService<AppDbContext>();
    }

    public AppDbContext Context { get; }

    public void Dispose()
    {
        RemoveData(JobEntity.TableName);
    }

    private void RemoveData(string tableName)
    {
        string sql = $"""
                          IF EXISTS (SELECT 1 FROM [{tableName}])
                          BEGIN
                              DELETE FROM [{tableName}];
                              DBCC CHECKIDENT('[{tableName}]', RESEED, 0);
                          END
                      """;
        Context.Database.ExecuteSqlRaw(sql);
    }
}
