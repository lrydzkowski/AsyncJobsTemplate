namespace AsyncJobsTemplate.Core.Models;

public class JobFile
{
    public byte[] Data { get; init; } = [];

    public string ContentType { get; init; } = "";

    public string FileName { get; init; } = "";
}
