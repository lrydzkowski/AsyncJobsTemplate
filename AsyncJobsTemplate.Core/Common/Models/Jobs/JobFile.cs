namespace AsyncJobsTemplate.Core.Common.Models.Jobs;

public class JobFile
{
    public Stream? Content { get; init; }

    public string ContentType { get; init; } = "";

    public string FileName { get; init; } = "";
}
