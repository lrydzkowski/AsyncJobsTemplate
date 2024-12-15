using AsyncJobsTemplate.Core.Jobs.Job3.Models;
using AsyncJobsTemplate.Core.Jobs.Job3.Services;
using AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi.Dtos;

namespace AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi.Services;

internal class TodoRepository : ITodoRepository
{
    private readonly ITodoClient _todoClient;

    public TodoRepository(ITodoClient todoClient)
    {
        _todoClient = todoClient;
    }

    public async Task<Todo?> GetTodoAsync(int todoId, CancellationToken cancellationToken)
    {
        GetTodoResponseDto? response = await _todoClient.GetTodoAsync(todoId, cancellationToken);
        if (response == null)
        {
            return null;
        }

        return new Todo
        {
            Id = response.Id,
            UserId = response.UserId,
            Title = response.Title,
            Completed = response.Completed
        };
    }
}
