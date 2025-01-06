using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using AsyncJobsTemplate.Core.Common.Options;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;

internal static class JobsBuilder
{
    public static WebApplicationFactory<Program> DisableJobsSleep(this WebApplicationFactory<Program> webApiFactory)
    {
        return webApiFactory.WithCustomOptions(
            new Dictionary<string, string?>
            {
                [$"{Job1Options.Position}:{nameof(Job1Options.Sleep)}"] = TimeSpan.Zero.ToString(),
                [$"{Job2Options.Position}:{nameof(Job2Options.Sleep)}"] = TimeSpan.Zero.ToString()
            }
        );
    }

    public static WebApplicationFactory<Program> WithJobsQueueMock(
        this WebApplicationFactory<Program> webApiFactory,
        out IJobsQueue jobsQueue
    )
    {
        jobsQueue = Substitute.For<IJobsQueue>();

        return webApiFactory.ReplaceService(jobsQueue, ServiceLifetime.Scoped);
    }
}
