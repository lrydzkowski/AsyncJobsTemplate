﻿namespace AsyncJobsTemplate.Shared.Services;

public interface IDateTimeProvider
{
    DateTime Now { get; }

    DateTime UtcNow { get; }
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;

    public DateTime UtcNow => DateTime.UtcNow;
}
