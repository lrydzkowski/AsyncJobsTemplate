using System.Text.Json.Nodes;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Core.Commands.TriggerJob.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AsyncJobsTemplate.Core.Commands.TriggerJob;

public class TriggerJobCommand : IRequest<TriggerJobResult>
{
    public TriggerJobRequest Request { get; init; } = new();
}

public class TriggerJobResult
{
    public Guid JobId { get; init; }
}

public class TriggerJobRequest
{
    public string? JobCategoryName { get; init; }

    public IFormFile? File { get; init; }

    public JsonObject? Data { get; init; }
}

public class TriggerJobCommandHandler : IRequestHandler<TriggerJobCommand, TriggerJobResult>
{
    private readonly IJobsFileStorage _jobFileStorage;
    private readonly IJobsQueue _jobsQueue;
    private readonly IJobsRepository _jobsRepository;

    public TriggerJobCommandHandler(
        IJobsFileStorage jobFileStorage,
        IJobsRepository jobsRepository,
        IJobsQueue jobsQueue
    )
    {
        _jobFileStorage = jobFileStorage;
        _jobsRepository = jobsRepository;
        _jobsQueue = jobsQueue;
    }

    public async Task<TriggerJobResult> Handle(TriggerJobCommand command, CancellationToken cancellationToken)
    {
        TriggerJobRequest request = command.Request;

        Guid jobId = Guid.NewGuid();

        string? inputFileReference = await SaveFileAsync(cancellationToken, request, jobId);
        await CreateJobAsync(cancellationToken, jobId, request, inputFileReference);
        await _jobsQueue.SendMessageAsync(jobId, cancellationToken);

        TriggerJobResult result = new()
        {
            JobId = jobId
        };

        return result;
    }

    private async Task<string?> SaveFileAsync(
        CancellationToken cancellationToken,
        TriggerJobRequest request,
        Guid jobId
    )
    {
        string? inputFileReference = null;
        if (request.File is not null)
        {
            SaveFileResult saveFileResult = await _jobFileStorage.SaveFileAsync(
                jobId.ToString(),
                request.File,
                cancellationToken
            );
            inputFileReference = saveFileResult.FileReference;
        }

        return inputFileReference;
    }

    private async Task CreateJobAsync(
        CancellationToken cancellationToken,
        Guid jobId,
        TriggerJobRequest request,
        string? inputFileReference
    )
    {
        JobToCreate jobToCreate = new()
        {
            JobId = jobId,
            InputData = request.Data,
            InputFileReference = inputFileReference
        };
        await _jobsRepository.CreateJobAsync(jobToCreate, cancellationToken);
    }
}
