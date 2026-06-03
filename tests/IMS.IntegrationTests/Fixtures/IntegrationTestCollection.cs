namespace IMS.IntegrationTests.Fixtures;

[CollectionDefinition(Name)]
public class IntegrationTestCollection
    : ICollectionFixture<PostgresIntegrationTestFixture>
{
    public const string Name = "IMS integration tests";
}
