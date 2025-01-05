using AsyncJobsTemplate.Shared.Models.Context;

namespace AsyncJobsTemplate.WebApi.Services;

public interface IRequestContextProvider
{
    RequestContext GetContext();
}

internal class RequestContextProvider
    : IRequestContextProvider
{
    private readonly IUserEmailProvider _userEmailProvider;
    private RequestContext? _currentContext;

    public RequestContextProvider(IUserEmailProvider userEmailProvider)
    {
        _userEmailProvider = userEmailProvider;
    }

    public RequestContext GetContext()
    {
        if (_currentContext is not null)
        {
            return _currentContext;
        }

        string? userEmail = _userEmailProvider.GetUserEmailFromClaims() ?? "";
        _currentContext = new RequestContext
        {
            User = new UserContext
            {
                UserEmail = userEmail
            }
        };

        return _currentContext;
    }
}
