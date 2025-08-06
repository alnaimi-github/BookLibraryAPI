using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;
using Xunit;

namespace BookLibraryAPI.IntegrationTests
{
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        protected readonly WebApplicationFactory<BookLibraryAPI.Presentation.IApiMarker> Factory;
        protected readonly PostgreSqlContainer PgContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        protected IntegrationTestBase(WebApplicationFactory<BookLibraryAPI.Presentation.IApiMarker> factory)
        {
            Factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    var connStr = PgContainer.GetConnectionString();
                    config.AddInMemoryCollection(new[]
                    {
                        new KeyValuePair<string, string?>("ConnectionStrings:LibraryDbConnection", connStr)
                    });
                });
            });
        }

        public async Task InitializeAsync() => await PgContainer.StartAsync();
        public async Task DisposeAsync() => await PgContainer.DisposeAsync();
    }
}
