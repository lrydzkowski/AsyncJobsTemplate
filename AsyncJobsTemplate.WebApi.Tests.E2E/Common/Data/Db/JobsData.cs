using System.Text.Json;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.Shared.Services;
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
                    CreatedAtUtc = job.CreatedAtUtc,
                    LastUpdatedAtUtc = job.LastUpdatedAtUtc
                }
            )
            .OrderBy(job => job.CreatedAtUtc)
            .ToListAsync();
    }

    public static async Task CreateJobAsync(
        TestContextScope scope,
        Guid jobId,
        string categoryName,
        string status = "Created"
    )
    {
        var inputData = new
        {
            Key1 = "Some data",
            Key2 = 123,
            Key3 = true,
            Key4 = new { Key5 = "Value1", Key6 = DateTime.UtcNow }
        };
        string serializedInputData = JsonSerializer.Serialize(inputData, Serializer.Options);
        JobEntity jobEntity = new()
        {
            JobId = jobId,
            JobCategoryName = categoryName,
            Status = status,
            InputData = serializedInputData,
            CreatedAtUtc = DateTime.UtcNow
        };
        ;
        scope.Db.Context.Jobs.Add(jobEntity);
        await scope.Db.Context.SaveChangesAsync();
    }
}
