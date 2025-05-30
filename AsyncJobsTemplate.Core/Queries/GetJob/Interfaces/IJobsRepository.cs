﻿using AsyncJobsTemplate.Core.Common.Models;

namespace AsyncJobsTemplate.Core.Queries.GetJob.Interfaces;

public interface IJobsRepository
{
    Task<Job?> GetJobAsync(string userEmail, Guid jobId, CancellationToken cancellationToken);
}
