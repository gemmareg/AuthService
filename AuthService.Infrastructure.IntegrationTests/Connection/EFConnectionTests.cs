using AuthService.Infrastructure.IntegrationTests.Fixtures;

namespace AuthService.Infrastructure.IntegrationTests.Connection
{
    public class EFConnectionTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public EFConnectionTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Should_Connect_To_Database()
        {
            var canConnect = await _fixture.DbContext.Database.CanConnectAsync();
            Assert.True(canConnect);
        }
    }
}
