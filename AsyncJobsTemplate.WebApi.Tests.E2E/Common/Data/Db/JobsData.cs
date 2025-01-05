using AsyncJobsTemplate.Infrastructure.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.Db;

internal static class JobsData
{
    public static async Task<IReadOnlyList<JobEntity>> GetJobsAsync(TestContextScope scope)
    {
        return await scope.Db.Context.Jobs.Select(
                job => new JobEntity
                {
                    RecId = job.RecId,
                    JobId = job.JobId,
                    JobCategoryName = job.JobCategoryName,
                    Status = job.Status,
                    InputData = job.InputData,
                    InputFileReference = job.InputFileReference,
                    OutputData = job.OutputData,
                    OutputFileReference = job.OutputFileReference,
                    Errors = job.Errors,
                    CreatedAt = job.CreatedAt,
                    LastUpdatedAt = job.LastUpdatedAt
                }
            )
            .OrderBy(job => job.CreatedAt)
            .ToListAsync();
    }

    public static async Task CreateJobAsync(TestContextScope scope, JobEntity jobEntity)
    {
        await CreateJobsAsync(scope, [jobEntity]);
    }

    public static async Task CreateJobsAsync(TestContextScope scope, List<JobEntity> jobEntities)
    {
        scope.Db.Context.Jobs.AddRange(jobEntities);
        await scope.Db.Context.SaveChangesAsync();
    }
}
