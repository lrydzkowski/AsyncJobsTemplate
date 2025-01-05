using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Core.Queries.GetJobs.Interfaces;
using AsyncJobsTemplate.Shared.Extensions;
using AsyncJobsTemplate.Shared.Models.Context;
using AsyncJobsTemplate.Shared.Models.Lists;
using AsyncJobsTemplate.Shared.Validators;
using MediatR;

namespace AsyncJobsTemplate.Core.Queries.GetJobs;

public class GetJobsQuery : IRequest<GetJobsResult>, IRequestContextOperation
{
    public ListRequest? Request { get; init; }

    public required RequestContext RequestContext { get; init; }
}

public class GetJobsResult
{
    public PaginatedList<Job> Jobs { get; init; } = new();
}

public class GetJobsQueryHandler : IRequestHandler<GetJobsQuery, GetJobsResult>
{
    private readonly GetJobsQueryValidator _getJobsValidator;
    private readonly IJobsRepository _jobsRepository;
    private readonly RequestContextValidator _requestContextValidator;

    public GetJobsQueryHandler(
        RequestContextValidator requestContextValidator,
        GetJobsQueryValidator getJobsValidator,
        IJobsRepository jobsRepository
    )
    {
        _requestContextValidator = requestContextValidator;
        _getJobsValidator = getJobsValidator;
        _jobsRepository = jobsRepository;
    }

    public async Task<GetJobsResult> Handle(GetJobsQuery query, CancellationToken cancellationToken)
    {
        await _requestContextValidator.ValidateAndThrowIfInvalidAsync(query, cancellationToken);
        await _getJobsValidator.ValidateAndThrowIfInvalidAsync(query, cancellationToken);

        ListParameters listParameters = new()
        {
            Pagination = new Pagination
            {
                Page = query.Request?.Page ?? 1,
                PageSize = query.Request?.PageSize ?? 100
            }
        };
        PaginatedList<Job> jobs = await _jobsRepository.GetJobsAsync(
            query.RequestContext.User.UserEmail,
            listParameters,
            cancellationToken
        );
        GetJobsResult result = new()
        {
            Jobs = jobs
        };

        return result;
    }
}
