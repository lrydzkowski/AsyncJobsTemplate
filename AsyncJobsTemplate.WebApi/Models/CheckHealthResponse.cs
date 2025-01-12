namespace AsyncJobsTemplate.WebApi.Models;

public class CheckHealthResponse
{
    public IReadOnlyDictionary<string, HealthReportEntryDto>? Entries { get; init; }
    public string Status { get; init; } = "";
    public TimeSpan TotalDuration { get; init; }
}

public class HealthReportEntryDto
{
    public IReadOnlyDictionary<string, object>? Data { get; init; }
    public string? Description { get; init; }
    public TimeSpan Duration { get; init; }
    public HealthReportEntryExceptionInfoDto? ExceptionInfo { get; init; }
    public string Status { get; init; } = "";
    public IEnumerable<string> Tags { get; init; } = [];
}

public class HealthReportEntryExceptionInfoDto
{
    public string? Message { get; init; }
    public string? StackTrace { get; init; }
}
