using Microsoft.EntityFrameworkCore;
using System.Reflection;
using AsyncJobsTemplate.Infrastructure.Db.Entities;

namespace AsyncJobsTemplate.Infrastructure.Db;

internal class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<JobEntity> Jobs => Set<JobEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
