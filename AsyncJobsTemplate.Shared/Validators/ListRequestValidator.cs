using AsyncJobsTemplate.Shared.Models.Lists;
using FluentValidation;

namespace AsyncJobsTemplate.Shared.Validators;

public class ListRequestValidator : AbstractValidator<ListRequest>
{
    public ListRequestValidator()
    {
        RuleFor(request => request)?.NotNull();
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1)
            .When(request => request != null)
            .WithName(nameof(ListRequest.Page));
        RuleFor(request => request.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(200)
            .When(request => request != null)
            .WithName(nameof(ListRequest.PageSize));
    }
}
