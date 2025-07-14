using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Api;

internal static class EndpointHelpers
{
    private const string DefaultPathParameterPlaceholder = "test";

    public static IReadOnlyList<EndpointInfo> GetEndpointsWithAuth(
        EndpointDataSource endpointDataSource,
        string? policyName = null,
        IReadOnlyList<EndpointInfo>? ignoredEndpoints = null
    )
    {
        ignoredEndpoints ??= [];
        List<EndpointInfo> endpoints = endpointDataSource.Endpoints
            .Where(endpoint => endpoint is RouteEndpoint)
            .Cast<RouteEndpoint>()
            .Where(endpoint => HasAuthPolicy(endpoint, policyName))
            .SelectMany(MapToEndpointsInfo)
            .Where(endpoint => FilterIgnoredEndpoints(ignoredEndpoints, endpoint))
            .OrderBy(endpoint => endpoint.Path)
            .ThenBy(endpoint => endpoint.HttpMethod.ToString())
            .ToList();

        return endpoints;
    }

    private static bool HasAuthPolicy(RouteEndpoint endpoint, string? policyName)
    {
        if (endpoint.Metadata.OfType<AllowAnonymousAttribute>().Any())
        {
            return false;
        }

        if (policyName is null)
        {
            return true;
        }

        return endpoint.Metadata.OfType<AuthorizeAttribute>()
            .Any(x => x.Policy?.Equals(policyName, StringComparison.InvariantCultureIgnoreCase) == true);
    }

    private static IEnumerable<EndpointInfo> MapToEndpointsInfo(RouteEndpoint endpoint)
    {
        List<EndpointInfo> endpointsInfo = [];
        IReadOnlyList<string> httpMethods =
            endpoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods ?? [];
        foreach (string httpMethod in httpMethods)
        {
            string path = endpoint.RoutePattern.RawText ?? "";
            foreach (RoutePatternParameterPart parameter in endpoint.RoutePattern.Parameters)
            {
                path = path.Replace("{" + parameter.Name + "}", DefaultPathParameterPlaceholder);
            }

            if (!path.StartsWith('/'))
            {
                path = '/' + path;
            }

            endpointsInfo.Add(
                new EndpointInfo
                {
                    Path = path,
                    HttpMethod = new HttpMethod(httpMethod)
                }
            );
        }

        return endpointsInfo;
    }

    private static bool FilterIgnoredEndpoints(IReadOnlyList<EndpointInfo> ignoredEndpoints, EndpointInfo endpoint)
    {
        return !ignoredEndpoints.Any(ignoredEndpoint =>
            ignoredEndpoint.Path.Equals(endpoint.Path, StringComparison.InvariantCultureIgnoreCase)
            && ignoredEndpoint.HttpMethod == endpoint.HttpMethod
        );
    }
}
