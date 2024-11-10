using AsyncJobsTemplate.WebApi.Tests.E2E.Common.WebApplication;

namespace AsyncJobsTemplate.WebApi.Tests.E2E.Common.TestCollections;

[CollectionDefinition(CollectionName)]
public class MainTestsCollection : ICollectionFixture<WebApiFactory>
{
    public const string CollectionName = "E2E.Main";
}
