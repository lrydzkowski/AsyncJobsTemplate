using System.Security.Claims;
using System.Text.Encodings.Web;
using AsyncJobsTemplate.Shared.Extensions;
using AsyncJobsTemplate.WebApi.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.WebApi.Authentication;

internal class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationSchemeOptions>
{
    public const string ApiKeyHeaderName = "api-key";

    private readonly string _apiKey;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IOptions<InternalEndpointsOptions> internalEndpointsOptions
    ) : base(options, logger, encoder)
    {
        _apiKey = internalEndpointsOptions.Value.ApiKey;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? apiKey = Request.Headers[ApiKeyHeaderName];
        if (!_apiKey.EqualsIgnoreCase(apiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Wrong API key."));
        }

        ClaimsPrincipal claimsPrincipal = new();
        ClaimsIdentity claimsIdentity = new("api-key");
        claimsPrincipal.AddIdentity(claimsIdentity);
        AuthenticationTicket authTicket = new(claimsPrincipal, ApiKeyAuthenticationSchemeOptions.Name);

        return Task.FromResult(AuthenticateResult.Success(authTicket));
    }
}
