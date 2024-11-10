namespace AsyncJobsTemplate.Core.Common.Models.Lists;

public class ListParameters
{
    public Pagination Pagination { get; init; } = new();
}

public class Pagination
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 100;
}
