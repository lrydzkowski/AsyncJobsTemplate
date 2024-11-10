using AsyncJobsTemplate.Core.Validators;
using FluentValidation;

namespace AsyncJobsTemplate.Core.Queries.GetJobs;

public class GetJobsQueryValidator : AbstractValidator<GetJobsQuery>
{
    public GetJobsQueryValidator(ListRequestValidator listRequestValidator)
    {
        RuleFor(query => query)?.NotNull();
        RuleFor(query => query.Request)?.SetValidator(listRequestValidator!)?.When(query => query != null);
    }
}
