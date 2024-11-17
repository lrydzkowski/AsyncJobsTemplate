using System.Text;
using AsyncJobsTemplate.WebApi.Options;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace AsyncJobsTemplate.WebApi.Middlewares;

public static class SwaggerBasicAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseSwaggerBasicAuth(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SwaggerBasicAuthMiddleware>();
    }
}

internal class SwaggerBasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SwaggerOptions _options;

    public SwaggerBasicAuthMiddleware(RequestDelegate next, IOptions<SwaggerOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next.Invoke(context);

            return;
        }

        if (string.IsNullOrWhiteSpace(_options.Username))
        {
            await _next.Invoke(context);

            return;
        }

        string? authHeader = context.Request.Headers[HeaderNames.Authorization];
        if (authHeader is null || !authHeader.StartsWith("Basic "))
        {
            SetUnauthorized(context);

            return;
        }

        string[] headerValueParts = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (headerValueParts.Length < 2)
        {
            SetUnauthorized(context);

            return;
        }

        string encodedValue = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
        string decodedValue = Encoding.UTF8.GetString(Convert.FromBase64String(encodedValue));
        string[] decodedValueParts = decodedValue.Split(':');
        if (decodedValueParts.Length != 2)
        {
            SetUnauthorized(context);

            return;
        }

        string username = decodedValueParts[0];
        string password = decodedValueParts[1];
        if (!IsAuthorized(username, password))
        {
            SetUnauthorized(context);

            return;
        }

        await _next.Invoke(context);
    }

    private void SetUnauthorized(HttpContext context)
    {
        context.Response.Headers.WWWAuthenticate = "Basic";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    }

    private bool IsAuthorized(string username, string password)
    {
        return username.Equals(_options.Username, StringComparison.InvariantCultureIgnoreCase)
               && password.Equals(_options.Password);
    }
}
