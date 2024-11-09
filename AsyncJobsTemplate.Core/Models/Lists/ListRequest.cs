using Microsoft.AspNetCore.Mvc;

namespace AsyncJobsTemplate.Core.Models.Lists;

public class ListRequest
{
    [FromQuery(Name = "page")] public int Page { get; init; } = 1;

    [FromQuery(Name = "pageSize")] public int PageSize { get; init; } = 100;
}
