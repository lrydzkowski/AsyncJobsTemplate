using AsyncJobsTemplate.WebApi.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AsyncJobsTemplate.WebApi.Mappers;

public interface ICheckHealthResponseMapper
{
    CheckHealthResponse? Map(HealthReport? healthReport);
}

internal class CheckHealthResponseMapper
    : ICheckHealthResponseMapper
{
    public CheckHealthResponse? Map(HealthReport? healthReport)
    {
        if (healthReport == null)
        {
            return null;
        }

        return new CheckHealthResponse
        {
            Entries = healthReport.Entries.ToDictionary(
                entry => entry.Key,
                entry => new HealthReportEntryDto
                {
                    Data = entry.Value.Data,
                    Description = entry.Value.Description,
                    Duration = entry.Value.Duration,
                    ExceptionInfo = entry.Value.Exception != null
                        ? new HealthReportEntryExceptionInfoDto
                        {
                            Message = entry.Value.Exception.Message,
                            StackTrace = entry.Value.Exception.StackTrace
                        }
                        : null,
                    Status = entry.Value.Status.ToString(),
                    Tags = entry.Value.Tags
                }
            ),
            Status = healthReport.Status.ToString(),
            TotalDuration = healthReport.TotalDuration
        };
    }
}
