using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;

internal static class QueueBuilder
{
    public static IJobsQueue? JobsQueue;

    public static WebApplicationFactory<Program> MockJobsQueue(this WebApplicationFactory<Program> webApiFactory)
    {
        JobsQueue = Substitute.For<IJobsQueue>()!;

        return webApiFactory.ReplaceService(JobsQueue, ServiceLifetime.Scoped);
    }
}
