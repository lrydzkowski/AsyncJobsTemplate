using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using AsyncJobsTemplate.Shared.Services;

namespace AsyncJobsTemplate.Shared.Extensions;

public static class HttpMessageExtensions
{
    public static void CreateContent<T>(this HttpRequestMessage request, T payload)
    {
        string serializedPayload = JsonSerializer.Serialize(payload, Serializer.Options);
        request.Content = new StringContent(
            serializedPayload,
            Encoding.UTF8,
            new MediaTypeHeaderValue(MediaTypeNames.Application.Json)
        );
    }

    public static async Task<T?> GetResponseAsync<T>(this HttpResponseMessage response)
    {
        string message = await response.GetResponseMessageAsync();
        T? payload = JsonSerializer.Deserialize<T>(message, Serializer.Options);

        return payload;
    }

    public static async Task<string> GetResponseMessageAsync(this HttpResponseMessage response)
    {
        return await response.Content.ReadAsStringAsync();
    }
}
