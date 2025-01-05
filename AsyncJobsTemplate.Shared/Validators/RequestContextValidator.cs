using AsyncJobsTemplate.Shared.Models.Context;
using FluentValidation;

namespace AsyncJobsTemplate.Shared.Validators;

public class RequestContextValidator : AbstractValidator<IRequestContextOperation>
{
    public RequestContextValidator(UserContextValidator userContextValidator)
    {
        RuleFor(x => x.RequestContext.User).SetValidator(userContextValidator);
    }
}

public class UserContextValidator : AbstractValidator<UserContext>
{
    public UserContextValidator()
    {
        RuleFor(x => x.UserEmail).EmailAddress().WithName(nameof(UserContext.UserEmail));
    }
}
