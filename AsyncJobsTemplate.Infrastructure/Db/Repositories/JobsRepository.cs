﻿using AsyncJobsTemplate.Core.Commands.RunJob.Models;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Infrastructure.Db.Entities;
using AsyncJobsTemplate.Infrastructure.Db.Mappers;
using AsyncJobsTemplate.Shared.Models.Lists;
using AsyncJobsTemplate.Shared.Services;
using Microsoft.EntityFrameworkCore;
using IJobsRepositoryGetJob = AsyncJobsTemplate.Core.Queries.GetJob.Interfaces.IJobsRepository;
using IJobsRepositoryGetJobs = AsyncJobsTemplate.Core.Queries.GetJobs.Interfaces.IJobsRepository;
using IJobsRepositoryRunJob = AsyncJobsTemplate.Core.Commands.RunJob.Interfaces.IJobsRepository;
using IJobsRepositoryTriggerJob = AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces.IJobsRepository;
using IJobsRepositoryDownloadJobFile = AsyncJobsTemplate.Core.Queries.DownloadJobFile.Interfaces.IJobsRepository;
using JobError = AsyncJobsTemplate.Infrastructure.Db.Models.JobError;
using JobErrorCore = AsyncJobsTemplate.Core.Common.Models.JobError;

namespace AsyncJobsTemplate.Infrastructure.Db.Repositories;

internal class JobsRepository
    : IJobsRepositoryTriggerJob,
        IJobsRepositoryRunJob,
        IJobsRepositoryGetJob,
        IJobsRepositoryGetJobs,
        IJobsRepositoryDownloadJobFile
{
    private readonly AppDbContext _appDbContext;
    private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
    private readonly IJobMapper _jobMapper;
    private readonly ISerializer _serializer;

    public JobsRepository(
        AppDbContext appDbContext,
        IDateTimeOffsetProvider dateTimeOffsetProvider,
        IJobMapper jobMapper,
        ISerializer serializer
    )
    {
        _appDbContext = appDbContext;
        _dateTimeOffsetProvider = dateTimeOffsetProvider;
        _jobMapper = jobMapper;
        _serializer = serializer;
    }

    public async Task<Job?> GetJobAsync(string userEmail, Guid jobId, CancellationToken cancellationToken)
    {
        JobEntity? jobEntity = await _appDbContext.Jobs.AsNoTracking()
            .FirstOrDefaultAsync(
                jobEntity => jobEntity.UserEmail == userEmail && jobEntity.JobId == jobId,
                cancellationToken
            );
        if (jobEntity is null)
        {
            return null;
        }

        Job job = BuildJob(jobEntity);

        return job;
    }

    public async Task<PaginatedList<Job>> GetJobsAsync(
        string userEmail,
        ListParameters listParameters,
        CancellationToken cancellationToken
    )
    {
        IQueryable<JobEntity> query = _appDbContext.Jobs.AsNoTracking()
            .Select(jobEntity => jobEntity)
            .Where(jobEntity => jobEntity.UserEmail == userEmail);
        List<JobEntity> jobEntities = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((listParameters.Pagination.Page - 1) * listParameters.Pagination.PageSize)
            .Take(listParameters.Pagination.PageSize)
            .ToListAsync(cancellationToken);
        int count = await query.CountAsync(cancellationToken);

        PaginatedList<Job> paginatedJobs = new()
        {
            Data = BuildJobs(jobEntities),
            Count = count
        };

        return paginatedJobs;
    }

    public async Task<Job?> GetJobAsync(Guid jobId, CancellationToken cancellationToken)
    {
        JobEntity? jobEntity = await _appDbContext.Jobs.AsNoTracking()
            .FirstOrDefaultAsync(
                jobEntity => jobEntity.JobId == jobId,
                cancellationToken
            );
        if (jobEntity is null)
        {
            return null;
        }

        Job job = BuildJob(jobEntity);

        return job;
    }

    public async Task UpdateJobAsync(JobToUpdate jobToUpdate, CancellationToken cancellationToken)
    {
        string? serializedOutputData = jobToUpdate.OutputData is null
            ? null
            : _serializer.Serialize(jobToUpdate.OutputData);
        string? serializedErrors = jobToUpdate.Errors is null
            ? null
            : _serializer.Serialize(_jobMapper.Map(jobToUpdate.Errors));

        await _appDbContext.Jobs.Where(jobEntity => jobEntity.JobId == jobToUpdate.JobId)
            .ExecuteUpdateAsync(
                x => x.SetProperty(jobEntity => jobEntity.Status, jobToUpdate.Status.ToString())
                    .SetProperty(jobEntity => jobEntity.OutputData, serializedOutputData)
                    .SetProperty(jobEntity => jobEntity.OutputFileReference, jobToUpdate.OutputFileReference)
                    .SetProperty(jobEntity => jobEntity.Errors, serializedErrors)
                    .SetProperty(jobEntity => jobEntity.LastUpdatedAt, _dateTimeOffsetProvider.UtcNow),
                cancellationToken
            );
    }

    public async Task SetJobStatusAsync(Guid jobId, JobStatus status, CancellationToken cancellationToken)
    {
        await _appDbContext.Jobs.Where(jobEntity => jobEntity.JobId == jobId)
            .ExecuteUpdateAsync(
                x => x.SetProperty(jobEntity => jobEntity.Status, status.ToString())
                    .SetProperty(jobEntity => jobEntity.LastUpdatedAt, _dateTimeOffsetProvider.UtcNow),
                cancellationToken
            );
    }

    public async Task<Job> CreateJobAsync(JobToCreate jobToCreate, CancellationToken cancellationToken)
    {
        JobEntity jobEntity = new()
        {
            UserEmail = jobToCreate.UserEmail,
            JobId = jobToCreate.JobId,
            JobCategoryName = jobToCreate.JobCategoryName,
            InputData = jobToCreate.InputData is null ? null : _serializer.Serialize(jobToCreate.InputData),
            InputFileReference = jobToCreate.InputFileReference,
            CreatedAt = _dateTimeOffsetProvider.UtcNow
        };
        _appDbContext.Add(jobEntity);
        await _appDbContext.SaveChangesAsync(cancellationToken);

        Job job = BuildJob(jobEntity);

        return job;
    }

    public async Task SaveErrorsAsync(Guid jobId, List<JobErrorCore> errors, CancellationToken cancellationToken)
    {
        string serializedErrors = _serializer.Serialize(_jobMapper.Map(errors));
        await _appDbContext.Jobs.Where(
                jobEntity => jobEntity.JobId == jobId
            )
            .ExecuteUpdateAsync(
                x => x.SetProperty(jobEntity => jobEntity.Errors, serializedErrors)
                    .SetProperty(jobEntity => jobEntity.LastUpdatedAt, _dateTimeOffsetProvider.UtcNow),
                cancellationToken
            );
    }

    private IReadOnlyList<Job> BuildJobs(List<JobEntity> jobEntities)
    {
        return jobEntities.Select(BuildJob).ToList();
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
                : _serializer.Deserialize<object>(jobEntity.InputData),
            InputFileReference = jobEntity.InputFileReference,
            OutputData = jobEntity.OutputData is null
                ? null
                : _serializer.Deserialize<object>(jobEntity.OutputData),
            OutputFileReference = jobEntity.OutputFileReference,
            Errors = jobEntity.Errors is null
                ? []
                : _jobMapper.Map(_serializer.Deserialize<List<JobError>>(jobEntity.Errors) ?? []),
            CreatedAt = jobEntity.CreatedAt,
            LastUpdatedAt = jobEntity.LastUpdatedAt
        };

        return job;
    }
}
