namespace AsyncJobsTemplate.Shared.Services;

public interface IDateTimeOffsetProvider
{
    DateTimeOffset Now { get; }

    DateTimeOffset UtcNow { get; }
}

public class DateTimeOffsetProvider : IDateTimeOffsetProvider
{
    public DateTimeOffset Now => DateTimeOffset.Now;

    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
