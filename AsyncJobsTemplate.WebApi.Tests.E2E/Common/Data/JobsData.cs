using System.Text.Json;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;

internal static class JobsData
{
    public static async Task<IReadOnlyList<JobEntity>> GetJobsAsync(DbContextScope dbScope)
    {
        return await dbScope.Context.Jobs.Select(
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
        DbContextScope dbScope,
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
        string serializedInputData = JsonSerializer.Serialize(
            inputData,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );
        JobEntity jobEntity = new()
        {
            JobId = jobId,
            JobCategoryName = categoryName,
            Status = status,
            InputData = serializedInputData,
            CreatedAtUtc = DateTime.UtcNow
        };
        ;
        dbScope.Context.Jobs.Add(jobEntity);
        await dbScope.Context.SaveChangesAsync();
    }
}
