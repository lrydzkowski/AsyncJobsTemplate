using AsyncJobsTemplate.Core.Common.Models;
using AsyncJobsTemplate.Core.Jobs.Job1;
using AsyncJobsTemplate.Infrastructure.Db.Entities;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;

internal class DbTestCaseData
{
    public List<JobEntity> Jobs { get; init; } =
    [
        new()
        {
            JobId = Guid.Parse("30d49143-3fee-4a5d-a150-1e18acb5f57d"),
            JobCategoryName = Job1Handler.Name,
            Status = JobStatus.Created.ToString(),
            InputData = """
                        {
                          "key": "value"
                        }
                        """,
            InputFileReference = null,
            OutputData = null,
            OutputFileReference = null,
            Errors = null,
            CreatedAt = new DateTimeOffset(2024, 12, 1, 10, 0, 0, TimeSpan.Zero),
            LastUpdatedAt = null,
            UserEmail = "test@asyncjobstemplate.com"
        }
    ];
}
