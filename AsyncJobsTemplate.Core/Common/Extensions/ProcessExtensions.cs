using AsyncJobsTemplate.Core.Common.Models.Jobs;
using Microsoft.Extensions.Logging;

namespace AsyncJobsTemplate.Core.Common.Extensions;

internal static class ProcessExtensions
{
    public static void HandleError(
        this ProcessContextBase process,
        ILogger logger,
        string errorCode,
        string errorMessage,
        params object?[] args
    )
    {
        logger.LogError(errorMessage, args);
        process.AddError(errorCode, string.Format(errorMessage, args));
    }

    public static void HandleError(
        this ProcessContextBase process,
        ILogger logger,
        string errorCode,
        string errorMessage,
        Exception ex,
        params object?[] args
    )
    {
        logger.LogError(ex, errorMessage, args);
        process.AddError(errorCode, string.Format(errorMessage, args), ex);
    }
}
