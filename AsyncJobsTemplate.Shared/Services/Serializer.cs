using System.Text.Json;

namespace AsyncJobsTemplate.Shared.Services;

public interface ISerializer
{
    string Serialize(object? data);
    T? Deserialize<T>(string? data) where T : class;
}

public class Serializer
    : ISerializer
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new DateTimeJsonConverter() }
    };

    public string Serialize(object? data)
    {
        return JsonSerializer.Serialize(data, Options);
    }

    public T? Deserialize<T>(string? data) where T : class
    {
        return data is null ? null : JsonSerializer.Deserialize<T>(data, Options);
    }
}
