﻿namespace AsyncJobsTemplate.Core.Commands.TriggerJob.Models;

public class JobToCreate
{
    public string UserEmail { get; init; } = "";

    public Guid JobId { get; init; }

    public string JobCategoryName { get; init; } = "";

    public object? InputData { get; init; }

    public string? InputFileReference { get; init; }
}
