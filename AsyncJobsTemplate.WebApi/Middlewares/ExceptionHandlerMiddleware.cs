using System.Net.Mime;
using System.Text.Json;
using AsyncJobsTemplate.Shared.Models.Errors;
using FluentValidation;

namespace AsyncJobsTemplate.WebApi.Middlewares;

public static class ExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}

public class ExceptionHandlerMiddleware
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(exception, context);
        }
    }

    private async Task HandleExceptionAsync(Exception exception, HttpContext context)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;

        switch (exception)
        {
            case ValidationException validationException:
                await HandleValidationExceptionAsync(context, validationException);
                break;
            case TaskCanceledException { InnerException: not TimeoutException } taskCanceledException:
                HandleTaskCancelledException(context, taskCanceledException);
                break;
            case { } generalException:
                HandleGlobalException(context, generalException);
                break;
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException validationException)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        IEnumerable<ErrorInfo> errors = validationException.Errors?.Select(
                                                x => new ErrorInfo
                                                {
                                                    PropertyName = x.PropertyName ?? "",
                                                    ErrorMessage = x.ErrorMessage ?? "",
                                                    AttemptedValue = x.AttemptedValue ?? "",
                                                    ErrorCode = x.ErrorCode ?? ""
                                                }
                                            )
                                            .AsEnumerable()
                                        ?? [];
        await context.Response.WriteAsJsonAsync(errors, _jsonSerializerOptions);
    }

    private void HandleTaskCancelledException(HttpContext context, TaskCanceledException exception)
    {
        _logger.LogWarning(exception, "Task cancelled exception");
        context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
    }

    private void HandleGlobalException(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Something went wrong");
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    }
}
