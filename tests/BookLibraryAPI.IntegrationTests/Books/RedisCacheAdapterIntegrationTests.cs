using System.Threading.Tasks;
using BookLibraryAPI.Infrastructure.Adapters.Caching;
using FluentAssertions;
using StackExchange.Redis;
using Testcontainers.Redis;
using Xunit;

namespace BookLibraryAPI.IntegrationTests.Books;

public class RedisCacheAdapterIntegrationTests : IAsyncLifetime
{
    private readonly RedisContainer _redisContainer = new RedisBuilder().Build();
    private IConnectionMultiplexer _redis = null!;
    private RedisCacheAdapter _cache = null!;

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();
        _redis = await ConnectionMultiplexer.ConnectAsync(_redisContainer.GetConnectionString());
        _cache = new RedisCacheAdapter(_redis, null!); // Logger is not used in basic tests
    }

    public async Task DisposeAsync()
    {
        await _redisContainer.DisposeAsync();
        _redis.Dispose();
    }

    [Fact]
    public async Task SetAndGetAsync_Should_Store_And_Retrieve_Value()
    {
        // Arrange
        var key = "test-key";
        var value = new TestValue { Name = "RedisTest", Number = 42 };

        // Act
        await _cache.SetAsync(key, value);
        var result = await _cache.GetAsync<TestValue>(key);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("RedisTest");
        result.Number.Should().Be(42);
    }

    private class TestValue
    {
        public string Name { get; set; } = string.Empty;
        public int Number { get; set; }
    }
}
