﻿using AsyncJobsTemplate.Core.Models;

namespace AsyncJobsTemplate.Core.Queries.GetJob.Models;

internal class ProcessContext : ProcessContextBase
{
    public required string JobIdToParse { get; init; }

    public Guid? JobId { get; set; }

    public Job? Job { get; set; }
}