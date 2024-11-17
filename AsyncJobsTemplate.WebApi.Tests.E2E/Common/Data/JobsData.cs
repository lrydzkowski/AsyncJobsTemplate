using AsyncJobsTemplate.Infrastructure.Db;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;

internal static class JobsData
{
    public static async Task<IReadOnlyList<JobEntity>> GetJobsAsync(DbContextScope dbScope)
    {
        AppDbContext dbContext = dbScope.GetDbContext();
        return await dbContext.Jobs.OrderBy(job => job.CreatedAtUtc).ToListAsync();
    }
}
