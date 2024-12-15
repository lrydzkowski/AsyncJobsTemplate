using AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi.Dtos;
using AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi.Options;
using AsyncJobsTemplate.Shared.Extensions;
using Microsoft.Extensions.Options;

namespace AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi.Services;

internal interface ITodoClient
{
    Task<GetTodoResponseDto?> GetTodoAsync(
        int id,
        CancellationToken cancellationToken
    );
}

internal class TodoClient
    : ITodoClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonPlaceholderOptions _options;

    public TodoClient(IHttpClientFactory httpClientFactory, IOptions<JsonPlaceholderOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task<GetTodoResponseDto?> GetTodoAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        string path = _options.TodoPath.Replace("{id}", id.ToString());
        HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, path);

        HttpClient client = _httpClientFactory.CreateClient(nameof(JsonPlaceholderApiHttpClient));
        HttpResponseMessage responseMessage = await client.SendAsync(httpRequestMessage, cancellationToken);
        await responseMessage.ThrowIfNotSuccessAsync();
        GetTodoResponseDto? response = await responseMessage.GetResponseAsync<GetTodoResponseDto?>();

        return response;
    }
}
