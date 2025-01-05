using AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCases;
using Microsoft.AspNetCore.Mvc.Testing;
using WireMock.Server;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication.Infrastructure;

internal static class DependenciesBuilder
{
    public static WebApplicationFactory<Program> WithDependencies(
        this WebApplicationFactory<Program> webApiFactory,
        WireMockServer wireMockServer,
        ITestCaseData testCase
    )
    {
        webApiFactory = webApiFactory.WithCustomUserEmail(testCase.UserEmail);
        webApiFactory = webApiFactory.WithGetTodoData(wireMockServer, testCase);

        return webApiFactory;
    }
}
