namespace AsyncJobsTemplate.Infrastructure.JsonPlaceholderApi.Dtos;

internal class GetTodoResponseDto
{
    public int UserId { get; init; }

    public int Id { get; init; }

    public string Title { get; init; } = "";

    public bool Completed { get; init; }
}
