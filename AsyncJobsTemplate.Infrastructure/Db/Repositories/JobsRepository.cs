using System.Text.Json;
using System.Text.Json.Nodes;
using AsyncJobsTemplate.Core.Commands.RunJob.Models;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Core.Models;
using AsyncJobsTemplate.Core.Services;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.Infrastructure.Db.Mappers;
using Microsoft.EntityFrameworkCore;
using IJobsRepositoryGetJob = AsyncJobsTemplate.Core.Queries.GetJob.Interfaces.IJobsRepository;
using IJobsRepositoryRunJob = AsyncJobsTemplate.Core.Commands.RunJob.Interfaces.IJobsRepository;
using IJobsRepositoryTriggerJob = AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces.IJobsRepository;
using JobError = AsyncJobsTemplate.Infrastructure.Db.Models.JobError;
using JobErrorCore = AsyncJobsTemplate.Core.Models.JobError;

namespace AsyncJobsTemplate.Infrastructure.Db.Repositories;

internal class JobsRepository : IJobsRepositoryTriggerJob, IJobsRepositoryRunJob, IJobsRepositoryGetJob
{
    private readonly AppDbContext _appDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    private readonly IJobMapper _jobMapper;

    public JobsRepository(AppDbContext appDbContext, IDateTimeProvider dateTimeProvider, IJobMapper jobMapper)
    {
        _appDbContext = appDbContext;
        _dateTimeProvider = dateTimeProvider;
        _jobMapper = jobMapper;
    }

    public async Task<Job?> GetJobAsync(Guid jobId, CancellationToken cancellationToken)
    {
        JobEntity? jobEntity = await _appDbContext.Jobs.AsNoTracking()
            .FirstOrDefaultAsync(x => x.JobId == jobId, cancellationToken);
        if (jobEntity is null)
        {
            return null;
        }

        Job? job = BuildJob(jobEntity);

        return job;
    }

    public async Task UpdateJobAsync(JobToUpdate jobToUpdate, CancellationToken cancellationToken)
    {
        string? serializedOutputData =
            jobToUpdate.OutputData is null ? null : JsonSerializer.Serialize(jobToUpdate.OutputData);
        string serializedErrors = JsonSerializer.Serialize(_jobMapper.Map(jobToUpdate.Errors));

        await _appDbContext.Jobs.Where(x => x.JobId == jobToUpdate.JobId)
            .ExecuteUpdateAsync(
                x => x.SetProperty(y => y.Status, jobToUpdate.Status.ToString())
                    .SetProperty(y => y.OutputData, serializedOutputData)
                    .SetProperty(y => y.OutputFileReference, jobToUpdate.OutputFileReference)
                    .SetProperty(y => y.Errors, serializedErrors)
                    .SetProperty(y => y.LastUpdatedAtUtc, _dateTimeProvider.UtcNow),
                cancellationToken
            );
    }

    public async Task<Job> CreateJobAsync(JobToCreate jobToCreate, CancellationToken cancellationToken)
    {
        JobEntity jobEntity = new()
        {
            JobId = jobToCreate.JobId,
            JobCategoryName = jobToCreate.JobCategoryName,
            InputData = jobToCreate.InputData is null ? null : JsonSerializer.Serialize(jobToCreate.InputData),
            InputFileReference = jobToCreate.InputFileReference,
            CreatedAtUtc = _dateTimeProvider.UtcNow
        };
        _appDbContext.Add(jobEntity);
        await _appDbContext.SaveChangesAsync(cancellationToken);

        Job job = BuildJob(jobEntity);

        return job;
    }

    public async Task SaveErrorsAsync(Guid jobId, List<JobErrorCore> errors, CancellationToken cancellationToken)
    {
        string serializedErrors = JsonSerializer.Serialize(_jobMapper.Map(errors));
        await _appDbContext.Jobs.Where(x => x.JobId == jobId)
            .ExecuteUpdateAsync(
                x => x.SetProperty(y => y.Errors, serializedErrors)
                    .SetProperty(y => y.LastUpdatedAtUtc, _dateTimeProvider.UtcNow),
                cancellationToken
            );
    }

    public async Task SetJobStatusAsync(Guid jobId, JobStatus status, CancellationToken cancellationToken)
    {
        await _appDbContext.Jobs.Where(x => x.JobId == jobId)
            .ExecuteUpdateAsync(
                x => x.SetProperty(y => y.Status, status.ToString())
                    .SetProperty(y => y.LastUpdatedAtUtc, _dateTimeProvider.UtcNow),
                cancellationToken
            );
    }

    private Job BuildJob(JobEntity jobEntity)
    {
        bool parsingResult = Enum.TryParse(jobEntity.Status, out JobStatus status);
        if (!parsingResult)
        {
            throw new InvalidOperationException($"Job entity has an incorrect status = '{jobEntity.Status}'.");
        }

        Job job = new()
        {
            JobId = jobEntity.JobId,
            JobCategoryName = jobEntity.JobCategoryName,
            Status = status,
            InputData = jobEntity.InputData is null
                ? null
                : JsonSerializer.Deserialize<JsonObject>(jobEntity.InputData),
            InputFileReference = jobEntity.InputFileReference,
            OutputData = jobEntity.OutputData is null
                ? null
                : JsonSerializer.Deserialize<JsonObject>(jobEntity.OutputData),
            OutputFileReference = jobEntity.OutputFileReference,
            Errors = jobEntity.Errors is null
                ? []
                : _jobMapper.Map(JsonSerializer.Deserialize<List<JobError>>(jobEntity.Errors) ?? []),
            CreatedAtUtc = jobEntity.CreatedAtUtc,
            LastUpdatedAtUtc = jobEntity.LastUpdatedAtUtc
        };

        return job;
    }
}
