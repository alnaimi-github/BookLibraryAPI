using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BookLibraryAPI.IntegrationTests
{
    [CollectionDefinition("Integration")]
    public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFixture> { }

    public class IntegrationTestFixture : IAsyncLifetime, IDisposable
    {
        public WebApplicationFactory<BookLibraryAPI.Presentation.IApiMarker> Factory { get; private set; }
        private readonly Testcontainers.PostgreSql.PostgreSqlContainer _pgContainer;
        private readonly Testcontainers.Redis.RedisContainer _redisContainer;

        public IntegrationTestFixture()
        {
            _pgContainer = new Testcontainers.PostgreSql.PostgreSqlBuilder()
                .WithDatabase("testdb")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();
            _redisContainer = BookLibraryAPI.IntegrationTests.RedisTestContainer.Create();
        }

        public async Task InitializeAsync()
        {
            await _pgContainer.StartAsync();
            await _redisContainer.StartAsync();
            Factory = new WebApplicationFactory<BookLibraryAPI.Presentation.IApiMarker>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((context, config) =>
                    {
                        var connStr = _pgContainer.GetConnectionString();
                        var redisConn = _redisContainer.GetConnectionString();
                        config.AddInMemoryCollection(new[]
                        {
                            new KeyValuePair<string, string?>("ConnectionStrings:LibraryDbConnection", connStr),
                            new KeyValuePair<string, string?>("ConnectionStrings:Redis", redisConn)
                        });
                    });
                });
        }

        public async Task DisposeAsync()
        {
            await _pgContainer.DisposeAsync();
            await _redisContainer.DisposeAsync();
            Factory.Dispose();
        }

        public void Dispose() => DisposeAsync().GetAwaiter().GetResult();
    }
}
