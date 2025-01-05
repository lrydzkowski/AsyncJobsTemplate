using System.Security.Claims;

namespace AsyncJobsTemplate.WebApi.Services;

internal interface IUserEmailProvider
{
    string? GetUserEmailFromClaims();
}

internal class UserEmailProvider : IUserEmailProvider
{
    private readonly ClaimsPrincipal? _claimsPrincipal;

    public UserEmailProvider(IHttpContextAccessor httpContextAccessor)
    {
        _claimsPrincipal = httpContextAccessor.HttpContext?.User;
    }

    public string? GetUserEmailFromClaims()
    {
        if (_claimsPrincipal is null)
        {
            return null;
        }

        string? userEmail = _claimsPrincipal.FindFirstValue(ClaimTypes.Email)
                            ?? _claimsPrincipal.FindFirstValue("emails")
                            ?? _claimsPrincipal.FindFirstValue("preferred_username")
                            ?? _claimsPrincipal.FindFirstValue(ClaimTypes.Upn);

        return userEmail?.Trim().ToLowerInvariant();
    }
}
