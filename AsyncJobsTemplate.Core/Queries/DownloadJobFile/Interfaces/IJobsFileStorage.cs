﻿using AsyncJobsTemplate.Core.Common.Models;

namespace AsyncJobsTemplate.Core.Queries.DownloadJobFile.Interfaces;

public interface IJobsFileStorage
{
    Task<JobFile?> GetOutputFileAsync(Guid fileReference, CancellationToken cancellationToken);
}
