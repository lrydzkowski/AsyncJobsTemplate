using AsyncJobsTemplate.Core.Models;
using Microsoft.Extensions.Logging;

namespace AsyncJobsTemplate.Core.Extensions;

internal static class ProcessExtensions
{
    public static void HandleError(
        this IProcess process,
        ILogger logger,
        string errorCode,
        string errorMsg,
        params object?[] args
    )
    {
        logger.LogError(errorMsg, args);
        process.Errors.Add(
            new JobError { Message = errorMsg, ErrorCode = errorCode }
        );
    }

    public static void HandleError(
        this IProcess process,
        ILogger logger,
        string errorCode,
        Exception ex,
        string errorMsg,
        params object?[] args
    )
    {
        process.Errors.Add(
            new JobError { Message = ex.Message, Exception = ex, ErrorCode = errorCode }
        );
        logger.LogError(ex, errorMsg, args);
    }
}
