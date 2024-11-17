using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace AsyncJobsTemplate.Shared.Extensions;

public static class HttpMessageExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static void CreateContent<T>(this HttpRequestMessage request, T payload)
    {
        string serializedPayload = JsonSerializer.Serialize(payload, JsonSerializerOptions);
        request.Content = new StringContent(
            serializedPayload,
            Encoding.UTF8,
            new MediaTypeHeaderValue(MediaTypeNames.Application.Json)
        );
    }

    public static async Task<T?> GetResponseAsync<T>(this HttpResponseMessage response)
    {
        string message = await response.Content.ReadAsStringAsync();
        T? payload = JsonSerializer.Deserialize<T>(message, JsonSerializerOptions);

        return payload;
    }
}
