using System.Text.Json.Nodes;
using AsyncJobsTemplate.Core.Extensions;
using AsyncJobsTemplate.Core.Models;
using AsyncJobsTemplate.Core.Queries.GetJob.Interfaces;
using AsyncJobsTemplate.Core.Queries.GetJob.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AsyncJobsTemplate.Core.Queries.GetJob;

public class GetJobQuery : IRequest<GetJobResult>
{
    public GetJobQueryRequest Request { get; init; } = new();
}

public class GetJobResult
{
    public JobStatus Status { get; init; }

    public JsonObject? Data { get; init; }

    public string? File { get; init; }
}

public class GetJobQueryHandler : IRequestHandler<GetJobQuery, GetJobResult>
{
    private readonly IJobsRepository _jobsRepository;
    private readonly ILogger<GetJobQueryHandler> _logger;

    public GetJobQueryHandler(ILogger<GetJobQueryHandler> logger, IJobsRepository jobsRepository)
    {
        _logger = logger;
        _jobsRepository = jobsRepository;
    }

    public async Task<GetJobResult> Handle(GetJobQuery query, CancellationToken cancellationToken)
    {
        Process process = new();
        process = GetJobId(process, query);
        process = await GetJobAsync(process, cancellationToken);

        GetJobResult result = BuildResult(process);

        return result;
    }

    private Process GetJobId(Process process, GetJobQuery query)
    {
        string jobGuidToParse = query.Request.JobId ?? "";
        bool parsingResult = Guid.TryParse(jobGuidToParse, out Guid jobId);
        if (!parsingResult)
        {
            process.HandleError(
                _logger,
                JobErrorCodes.GetJobFailure,
                "An unexpected error has occured in parsing job guid = '{JobGuid}'",
                jobGuidToParse
            );

            return process;
        }

        process.JobId = jobId;

        return process;
    }

    private async Task<Process> GetJobAsync(Process process, CancellationToken cancellationToken)
    {
        if (process.HasErrors || process.JobId is null)
        {
            return process;
        }

        Job? job;

        try
        {
            job = await _jobsRepository.GetJobAsync((Guid)process.JobId, cancellationToken);
        }
        catch (Exception ex)
        {
            process.HandleError(
                _logger,
                JobErrorCodes.GetJobFailure,
                ex,
                "An unexpected error has occured in getting a job."
            );

            return process;
        }

        if (job is null)
        {
            process.HandleError(
                _logger,
                JobErrorCodes.GetJobFailure,
                "An unexpected error has occured in getting a job. Job doesn't exist."
            );

            return process;
        }

        process.Job = job;

        return process;
    }

    private static GetJobResult BuildResult(Process process)
    {
        GetJobResult result = new()
        {
            Status = process.Job?.Status ?? JobStatus.NotExist,
            Data = process.Job?.OutputData,
            File = process.Job?.OutputFileReference
        };

        return result;
    }
}
