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
    private readonly ISerializer _serializer;

    public JobsRepository(
        AppDbContext appDbContext,
        IDateTimeProvider dateTimeProvider,
        IJobMapper jobMapper,
        ISerializer serializer
    )
    {
        _appDbContext = appDbContext;
        _dateTimeProvider = dateTimeProvider;
        _jobMapper = jobMapper;
        _serializer = serializer;
    }

    public async Task<Job?> GetJobAsync(Guid jobId, CancellationToken cancellationToken)
    {
        JobEntity? jobEntity = await _appDbContext.Jobs.AsNoTracking()
            .FirstOrDefaultAsync(jobEntity => jobEntity.JobId == jobId, cancellationToken);
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
                    .SetProperty(jobEntity => jobEntity.LastUpdatedAtUtc, _dateTimeProvider.UtcNow),
                cancellationToken
            );
    }

    public async Task SetJobStatusAsync(Guid jobId, JobStatus status, CancellationToken cancellationToken)
    {
        await _appDbContext.Jobs.Where(jobEntity => jobEntity.JobId == jobId)
            .ExecuteUpdateAsync(
                x => x.SetProperty(jobEntity => jobEntity.Status, status.ToString())
                    .SetProperty(jobEntity => jobEntity.LastUpdatedAtUtc, _dateTimeProvider.UtcNow),
                cancellationToken
            );
    }

    public async Task<Job> CreateJobAsync(JobToCreate jobToCreate, CancellationToken cancellationToken)
    {
        JobEntity jobEntity = new()
        {
            JobId = jobToCreate.JobId,
            JobCategoryName = jobToCreate.JobCategoryName,
            InputData = jobToCreate.InputData is null ? null : _serializer.Serialize(jobToCreate.InputData),
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
        string serializedErrors = _serializer.Serialize(_jobMapper.Map(errors));
        await _appDbContext.Jobs.Where(
                jobEntity => jobEntity.JobId == jobId
            )
            .ExecuteUpdateAsync(
                x => x.SetProperty(jobEntity => jobEntity.Errors, serializedErrors)
                    .SetProperty(jobEntity => jobEntity.LastUpdatedAtUtc, _dateTimeProvider.UtcNow),
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
                : _serializer.Deserialize<object>(jobEntity.InputData),
            InputFileReference = jobEntity.InputFileReference,
            OutputData = jobEntity.OutputData is null
                ? null
                : _serializer.Deserialize<object>(jobEntity.OutputData),
            OutputFileReference = jobEntity.OutputFileReference,
            Errors = jobEntity.Errors is null
                ? []
                : _jobMapper.Map(_serializer.Deserialize<List<JobError>>(jobEntity.Errors) ?? []),
            CreatedAtUtc = jobEntity.CreatedAtUtc,
            LastUpdatedAtUtc = jobEntity.LastUpdatedAtUtc
        };

        return job;
    }
}
