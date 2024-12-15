using Microsoft.Net.Http.Headers;

namespace AsyncJobsTemplate.Shared.Models.Http;

public class HttpHeaderNames
{
    public const string OcpApimSubscriptionKey = "Ocp-Apim-Subscription-Key";

    public const string Access = "Access";

    public const string ApiKey = "api-key";

    public static readonly IReadOnlyList<string> HeadersToFilterFromLogs =
    [
        HeaderNames.Authorization, OcpApimSubscriptionKey, Access, ApiKey
    ];
}
