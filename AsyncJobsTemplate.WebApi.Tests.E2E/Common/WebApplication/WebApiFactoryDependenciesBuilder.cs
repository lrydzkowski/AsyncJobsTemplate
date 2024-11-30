using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.Data.Db;
using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;

internal static class WebApiFactoryDependenciesBuilder
{
    public static async Task<WebApplicationFactory<Program>> BuildAsync(
        this WebApplicationFactory<Program> webApiFactory,
        TestContextScope scope,
        ITestCaseData testCaseData
    )
    {
        return await MockDbDataAsync(webApiFactory, scope, testCaseData);
    }

    private static async Task<WebApplicationFactory<Program>> MockDbDataAsync(
        this WebApplicationFactory<Program> webApiFactory,
        TestContextScope scope,
        ITestCaseData testCaseData
    )
    {
        await JobsData.CreateJobAsync(scope, testCaseData.Data.Db.Jobs);

        return webApiFactory;
    }
}
