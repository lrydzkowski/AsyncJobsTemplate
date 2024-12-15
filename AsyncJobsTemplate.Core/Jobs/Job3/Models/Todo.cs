namespace AsyncJobsTemplate.Core.Jobs.Job3.Models;

public class Todo
{
    public int UserId { get; init; }

    public int Id { get; init; }

    public string Title { get; init; } = "";

    public bool Completed { get; init; }
}
