using AsyncJobsTemplate.Core.Extensions;
using AsyncJobsTemplate.Core.Models;
using AsyncJobsTemplate.Core.Models.Lists;
using AsyncJobsTemplate.Core.Queries.GetJobs.Interfaces;
using MediatR;

namespace AsyncJobsTemplate.Core.Queries.GetJobs;

public class GetJobsQuery : IRequest<GetJobsResult>
{
    public ListRequest? Request { get; init; }
}

public class GetJobsResult
{
    public PaginatedList<Job> Jobs { get; init; } = new();
}

public class GetJobsQueryHandler : IRequestHandler<GetJobsQuery, GetJobsResult>
{
    private readonly IJobsRepository _jobsRepository;
    private readonly GetJobsQueryValidator _validator;

    public GetJobsQueryHandler(GetJobsQueryValidator validator, IJobsRepository jobsRepository)
    {
        _validator = validator;
        _jobsRepository = jobsRepository;
    }

    public async Task<GetJobsResult> Handle(GetJobsQuery query, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowIfInvalidAsync(query, cancellationToken);

        ListParameters listParameters = new()
        {
            Pagination = new Pagination
            {
                Page = query.Request?.Page ?? 1,
                PageSize = query.Request?.PageSize ?? 100
            }
        };
        PaginatedList<Job> jobs = await _jobsRepository.GetJobsAsync(listParameters, cancellationToken);
        GetJobsResult result = new()
        {
            Jobs = jobs
        };

        return result;
    }
}
