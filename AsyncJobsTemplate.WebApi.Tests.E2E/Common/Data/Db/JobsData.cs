using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;
using Microsoft.EntityFrameworkCore;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.Db;

internal static class JobsData
{
    public static async Task<IReadOnlyList<JobEntity>> GetJobsAsync(this TestContextScope scope)
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
                    LastUpdatedAt = job.LastUpdatedAt,
                    UserEmail = job.UserEmail
                }
            )
            .OrderBy(job => job.CreatedAt)
            .ToListAsync();
    }

    public static async Task CreateJobsAsync(this TestContextScope scope, ITestCaseData testCase)
    {
        scope.Db.Context.Jobs.AddRange(testCase.Data.Db.Jobs);
        await scope.Db.Context.SaveChangesAsync();
    }
}
