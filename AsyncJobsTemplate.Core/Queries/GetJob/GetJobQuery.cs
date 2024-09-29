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
    public string Status { get; init; } = "";

    public JsonObject? OutputData { get; init; }

    public string? OutputFile { get; init; }
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
        Process process = new()
        {
            JobIdToParse = query.Request.JobId ?? ""
        };
        process = GetJobId(process);
        process = await GetJobAsync(process, cancellationToken);

        GetJobResult result = BuildResult(process);

        return result;
    }

    private Process GetJobId(Process process)
    {
        bool parsingResult = Guid.TryParse(process.JobIdToParse, out Guid jobId);
        if (!parsingResult)
        {
            process.HandleError(
                _logger,
                JobErrorCodes.GetJobFailure,
                "An unexpected error has occured in parsing job guid = '{JobGuid}'",
                process.JobIdToParse
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
            Status = process.Job?.Status.ToString() ?? JobStatus.NotExist.ToString(),
            OutputData = process.Job?.OutputData,
            OutputFile = process.Job?.OutputFileReference
        };

        return result;
    }
}
