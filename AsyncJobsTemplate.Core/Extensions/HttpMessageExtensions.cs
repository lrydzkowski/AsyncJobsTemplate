using System.Text.Json;

namespace AsyncJobsTemplate.Core.Extensions;

public static class HttpMessageExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task<T?> GetResponseAsync<T>(this HttpResponseMessage response)
    {
        string message = await response.Content.ReadAsStringAsync();
        T? payload = JsonSerializer.Deserialize<T>(message, JsonSerializerOptions);

        return payload;
    }
}
