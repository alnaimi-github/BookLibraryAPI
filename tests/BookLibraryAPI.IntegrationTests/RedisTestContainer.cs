using Testcontainers.Redis;

namespace BookLibraryAPI.IntegrationTests
{
    public static class RedisTestContainer
    {
        public static RedisContainer Create()
        {
            return new RedisBuilder()
                .WithImage("redis:7.0")
                .WithPortBinding(6379, true)
                .Build();
        }
    }
}
