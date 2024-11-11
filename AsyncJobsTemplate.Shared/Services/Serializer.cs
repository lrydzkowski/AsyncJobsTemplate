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
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public string Serialize(object? data)
    {
        return JsonSerializer.Serialize(data, _options);
    }

    public T? Deserialize<T>(string? data) where T : class
    {
        return data is null ? null : JsonSerializer.Deserialize<T>(data, _options);
    }
}
