using Microsoft.AspNetCore.Authentication;

namespace AsyncJobsTemplate.WebApi.Authentication;

internal class ApiKeyAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public const string Name = "ApiKeyAuthenticationScheme";
}
