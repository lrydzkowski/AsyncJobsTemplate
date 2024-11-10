namespace AsyncJobsTemplate.Core.Common.Models.Lists;

public class PaginatedList<TData>
{
    public IReadOnlyList<TData> Data { get; init; } = [];

    public long Count { get; init; } = 0;
}
