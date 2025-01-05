using AsyncJobsTemplate.Core.Common.Extensions;
using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Core.Queries.GetJob.Interfaces;
using AsyncJobsTemplate.Core.Queries.GetJob.Models;
using AsyncJobsTemplate.Shared.Extensions;
using AsyncJobsTemplate.Shared.Models.Context;
using AsyncJobsTemplate.Shared.Validators;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AsyncJobsTemplate.Core.Queries.GetJob;

public class GetJobQuery : IRequest<GetJobResult>, IRequestContextOperation
{
    public GetJobRequest Request { get; init; } = new();

    public required RequestContext RequestContext { get; init; }
}

public class GetJobRequest
{
    public string? JobId { get; init; }
}

public class GetJobResult
{
    public string Status { get; init; } = "";

    public object? OutputData { get; init; }

    public string? OutputFileReference { get; init; }
}

public class GetJobQueryHandler : IRequestHandler<GetJobQuery, GetJobResult>
{
    private readonly IJobsRepository _jobsRepository;
    private readonly ILogger<GetJobQueryHandler> _logger;
    private readonly RequestContextValidator _requestContextValidator;

    public GetJobQueryHandler(
        RequestContextValidator requestContextValidator,
        ILogger<GetJobQueryHandler> logger,
        IJobsRepository jobsRepository
    )
    {
        _requestContextValidator = requestContextValidator;
        _logger = logger;
        _jobsRepository = jobsRepository;
    }

    public async Task<GetJobResult> Handle(GetJobQuery query, CancellationToken cancellationToken)
    {
        await _requestContextValidator.ValidateAndThrowIfInvalidAsync(query, cancellationToken);

        ProcessContext process = new()
        {
            UserEmail = query.RequestContext.User.UserEmail,
            JobIdToParse = query.Request.JobId ?? ""
        };
        process = GetJobId(process);
        process = await GetJobAsync(process, cancellationToken);

        GetJobResult result = BuildResult(process);

        return result;
    }

    private ProcessContext GetJobId(ProcessContext process)
    {
        bool parsingResult = Guid.TryParse(process.JobIdToParse, out Guid jobId);
        if (!parsingResult)
        {
            string errorCode = JobErrorCodes.GetJobFailure;
            string errorMessage = "An unexpected error has occured in parsing a job GUID = '{0}'";
            process.HandleError(_logger, errorCode, errorMessage, process.JobIdToParse);

            return process;
        }

        process.JobId = jobId;

        return process;
    }

    private async Task<ProcessContext> GetJobAsync(ProcessContext process, CancellationToken cancellationToken)
    {
        if (process.HasErrors || process.JobId is null)
        {
            return process;
        }

        Job? job;

        try
        {
            job = await _jobsRepository.GetJobAsync(process.UserEmail, (Guid)process.JobId, cancellationToken);
        }
        catch (Exception ex)
        {
            string errorCode = JobErrorCodes.GetJobFailure;
            string errorMessage = "An unexpected error has occured in getting a job.";
            process.HandleError(_logger, errorCode, errorMessage, ex);

            return process;
        }

        if (job is null)
        {
            string errorCode = JobErrorCodes.GetJobFailure;
            string errorMessage = "An unexpected error has occured in getting a job. Job doesn't exist.";
            process.HandleError(_logger, errorCode, errorMessage);

            return process;
        }

        process.Job = job;

        return process;
    }

    private static GetJobResult BuildResult(ProcessContext process)
    {
        GetJobResult result = new()
        {
            Status = process.Job?.Status.ToString() ?? JobStatus.NotExist.ToString(),
            OutputData = process.Job?.OutputData,
            OutputFileReference = process.Job?.OutputFileReference
        };

        return result;
    }
}
