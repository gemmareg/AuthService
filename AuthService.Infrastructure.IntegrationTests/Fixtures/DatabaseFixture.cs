using AuthService.Infrastructure.Persistance.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.IntegrationTests.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        public AuthDbContext DbContext { get; }
        public DatabaseFixture()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var services = new ServiceCollection();

            services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(config.GetConnectionString("SqlServer")));
            var provider = services.BuildServiceProvider();

            DbContext = provider.GetRequiredService<AuthDbContext>();

            // Ensure database exists
            DbContext.Database.EnsureCreated();
        }
        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            DbContext.Dispose();
        }
    }
}
