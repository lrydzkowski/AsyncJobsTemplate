using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Core.Queries.DownloadJobFile.Interfaces;
using AsyncJobsTemplate.Core.Queries.DownloadJobFile.Models;
using MediatR;

namespace AsyncJobsTemplate.Core.Queries.DownloadJobFile;

public class DownloadJobFileQuery : IRequest<DownloadJobFileResult>
{
    public DownloadJobFileRequest Request { get; init; } = new();
}

public class DownloadJobFileResult
{
    public JobFile? File { get; init; }
}

public class DownloadJobFileQueryHandler : IRequestHandler<DownloadJobFileQuery, DownloadJobFileResult>
{
    private readonly IJobsFileStorage _jobsFileStorage;

    public DownloadJobFileQueryHandler(IJobsFileStorage jobsFileStorage)
    {
        _jobsFileStorage = jobsFileStorage;
    }

    public async Task<DownloadJobFileResult> Handle(DownloadJobFileQuery query, CancellationToken cancellationToken)
    {
        if (query.Request.FileReference is null)
        {
            return new DownloadJobFileResult();
        }

        bool parsingResult = Guid.TryParse(query.Request.FileReference, out Guid fileReference);
        if (!parsingResult)
        {
            return new DownloadJobFileResult();
        }

        JobFile? file = await _jobsFileStorage.GetOutputFileAsync(fileReference, cancellationToken);
        DownloadJobFileResult result = new()
        {
            File = file
        };

        return result;
    }
}
