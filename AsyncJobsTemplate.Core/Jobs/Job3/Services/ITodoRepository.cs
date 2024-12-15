using AsyncJobsTemplate.Core.Jobs.Job3.Models;

namespace AsyncJobsTemplate.Core.Jobs.Job3.Services;

public interface ITodoRepository
{
    Task<Todo?> GetTodoAsync(int todoId, CancellationToken cancellationToken);
}
