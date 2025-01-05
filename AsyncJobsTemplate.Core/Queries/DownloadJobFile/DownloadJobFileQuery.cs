using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Core.Queries.DownloadJobFile.Interfaces;
using AsyncJobsTemplate.Shared.Extensions;
using AsyncJobsTemplate.Shared.Models.Context;
using AsyncJobsTemplate.Shared.Validators;
using MediatR;

namespace AsyncJobsTemplate.Core.Queries.DownloadJobFile;

public class DownloadJobFileQuery : IRequest<DownloadJobFileResult>, IRequestContextOperation
{
    public DownloadJobFileRequest Request { get; init; } = new();

    public required RequestContext RequestContext { get; init; }
}

public class DownloadJobFileRequest
{
    public string? JobId { get; init; }
}

public class DownloadJobFileResult
{
    public JobFile? File { get; init; }
}

public class DownloadJobFileQueryHandler : IRequestHandler<DownloadJobFileQuery, DownloadJobFileResult>
{
    private readonly IJobsFileStorage _jobsFileStorage;
    private readonly IJobsRepository _jobsRepository;
    private readonly RequestContextValidator _requestContextValidator;

    public DownloadJobFileQueryHandler(
        RequestContextValidator requestContextValidator,
        IJobsRepository jobsRepository,
        IJobsFileStorage jobsFileStorage
    )
    {
        _requestContextValidator = requestContextValidator;
        _jobsRepository = jobsRepository;
        _jobsFileStorage = jobsFileStorage;
    }

    public async Task<DownloadJobFileResult> Handle(DownloadJobFileQuery query, CancellationToken cancellationToken)
    {
        await _requestContextValidator.ValidateAndThrowIfInvalidAsync(query, cancellationToken);

        if (query.Request.JobId is null)
        {
            return new DownloadJobFileResult();
        }

        bool parsingJobIdResult = Guid.TryParse(query.Request.JobId, out Guid jobId);
        if (!parsingJobIdResult)
        {
            return new DownloadJobFileResult();
        }

        Job? job = await _jobsRepository.GetJobAsync(query.RequestContext.User.UserEmail, jobId, cancellationToken);
        if (job?.OutputFileReference is null)
        {
            return new DownloadJobFileResult();
        }

        bool parsingOutputFileReferenceResult = Guid.TryParse(job.OutputFileReference, out Guid outputFileReference);
        if (!parsingOutputFileReferenceResult)
        {
            return new DownloadJobFileResult();
        }

        JobFile? file = await _jobsFileStorage.GetOutputFileAsync(outputFileReference, cancellationToken);
        DownloadJobFileResult result = new()
        {
            File = file
        };

        return result;
    }
}
