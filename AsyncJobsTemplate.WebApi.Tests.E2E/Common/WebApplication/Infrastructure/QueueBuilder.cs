using AsyncJobsTemplate.Core.Commands.TriggerJob.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;

internal static class QueueBuilder
{
    public static WebApplicationFactory<Program> MockJobsQueue(
        this WebApplicationFactory<Program> webApiFactory,
        IJobsQueue jobsQueue
    )
    {
        return webApiFactory.ReplaceService(jobsQueue, ServiceLifetime.Scoped);
    }
}
