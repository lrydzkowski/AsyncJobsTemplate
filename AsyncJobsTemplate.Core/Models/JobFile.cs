namespace AsyncJobsTemplate.Core.Models;

public class JobFile
{
    public Stream? Content { get; init; }

    public string ContentType { get; init; } = "";

    public string FileName { get; init; } = "";
}
