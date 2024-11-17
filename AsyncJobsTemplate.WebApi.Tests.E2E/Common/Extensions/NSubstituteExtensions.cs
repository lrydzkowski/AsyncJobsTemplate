using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Models;
using NSubstitute;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.Extensions;

internal static class NSubstituteExtensions
{
    public static IReadOnlyList<ReceivedMethodCall> GetReceivedMethodCalls<T>(this T substitute) where T : class
    {
        return substitute.ReceivedCalls()
                   ?.Select(
                       receivedCall => new ReceivedMethodCall
                       {
                           MethodName = receivedCall.GetMethodInfo().Name,
                           ReceivedArguments = receivedCall.GetArguments().ToList() ?? []
                       }
                   )
                   .ToList()
               ?? [];
    }
}
