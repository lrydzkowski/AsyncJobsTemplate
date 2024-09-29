using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Nodes;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Core.Models;
using AsyncJobsTemplate.Core.Services;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.Infrastructure.Db.Mappers;
using Microsoft.EntityFrameworkCore;
using JobErrorCore = AsyncJobsTemplate.Core.Models.JobError;
using JobError = AsyncJobsTemplate.Infrastructure.Db.Models.JobError;
using IJobsRepositoryGetJob = AsyncJobsTemplate.Core.Queries.GetJob.Interfaces.IJobsRepository;
using IJobsRepositoryRunJob = AsyncJobsTemplate.Core.Commands.RunJob.Interfaces.IJobsRepository;
using IJobsRepositoryTriggerJob = AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces.IJobsRepository;

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

    public async Task SetJobStatusAsync(Guid jobId, JobStatus status, CancellationToken cancellationToken)
    {
        JobEntity? jobEntity = await _appDbContext.Jobs.FirstOrDefaultAsync(x => x.JobId == jobId, cancellationToken);
        if (jobEntity is null)
        {
            return;
        }

        jobEntity.Status = status.ToString();
        await _appDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SetJobStatusAsync(
        Guid jobId,
        JobStatus status,
        List<JobErrorCore> errors,
        CancellationToken cancellationToken
    )
    {
        JobEntity? jobEntity = await _appDbContext.Jobs.FirstOrDefaultAsync(x => x.JobId == jobId, cancellationToken);
        if (jobEntity is null)
        {
            return;
        }

        SetErrors(jobEntity, errors);
        jobEntity.Status = status.ToString();

        await _appDbContext.SaveChangesAsync(cancellationToken);
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

        // TODO: improve it
        Job? job = await GetJobAsync(jobToCreate.JobId, cancellationToken);
        if (job is null)
        {
            throw new InvalidOperationException("Job doesn't exist.");
        }

        return job;
    }

    public async Task<Job?> SaveErrorsAsync(Guid jobId, List<JobErrorCore> errors, CancellationToken cancellationToken)
    {
        JobEntity? jobEntity = await _appDbContext.Jobs.FirstOrDefaultAsync(x => x.JobId == jobId, cancellationToken);
        if (jobEntity is null)
        {
            return null;
        }

        SetErrors(jobEntity, errors);
        await _appDbContext.SaveChangesAsync(cancellationToken);

        // TODO: improve it
        Job? job = await GetJobAsync(jobId, cancellationToken);
        if (job is null)
        {
            throw new InvalidOperationException("Job doesn't exist.");
        }

        return job;
    }

    private JobEntity SetErrors(JobEntity jobEntity, List<JobErrorCore> errors)
    {
        List<JobError> errorsToSave = _jobMapper.Map(errors);
        if (jobEntity.Errors is not null)
        {
            List<JobError> currentErrors = JsonSerializer.Deserialize<List<JobError>>(jobEntity.Errors) ?? [];
            errorsToSave.AddRange(currentErrors);
        }

        jobEntity.Errors = JsonSerializer.Serialize(errorsToSave);

        return jobEntity;
    }

    private Job? BuildJob(JobEntity jobEntity)
    {
        bool parsingResult = Enum.TryParse(jobEntity.Status, out JobStatus status);
        if (!parsingResult)
        {
            return null;
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
