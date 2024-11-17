using FluentValidation;
using FluentValidation.Results;

namespace AsyncJobsTemplate.Shared.Extensions;

public static class ValidationExtensions
{
    public static void ThrowIfInvalid(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            return;
        }

        throw new ValidationException(validationResult.Errors ?? []);
    }

    public static async Task ValidateAndThrowIfInvalidAsync<T>(
        this AbstractValidator<T> validator,
        T dataToValidate,
        CancellationToken cancellationToken
    )
    {
        (await validator.ValidateAsync(dataToValidate, cancellationToken)!).ThrowIfInvalid();
    }
}
