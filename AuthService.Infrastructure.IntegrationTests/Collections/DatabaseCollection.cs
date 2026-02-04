using AuthService.Infrastructure.IntegrationTests.Fixtures;

namespace AuthService.Infrastructure.IntegrationTests.Collections
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}
