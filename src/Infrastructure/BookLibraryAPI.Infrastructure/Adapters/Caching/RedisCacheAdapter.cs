using System.Text.Json;
using BookLibraryAPI.Core.Domain.Interfaces.Ports.Caching;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace BookLibraryAPI.Infrastructure.Adapters.Caching;

public class RedisCacheAdapter(IConnectionMultiplexer redis, ILogger<RedisCacheAdapter> logger)
    : ICachePort
{
    private readonly IDatabase _database = redis.GetDatabase();

    public async Task<T?> GetAsync<T>(
        string key, 
        CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            logger.LogDebug("Getting cached value for key: {Key}", key);
            
            var value = await _database.StringGetAsync(key);
            
            if (!value.HasValue)
            {
                logger.LogDebug("Cache miss for key: {Key}", key);
                return null;
            }

            var result = JsonSerializer.Deserialize<T>(value!);
            logger.LogDebug("Cache hit for key: {Key}", key);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting cached value for key: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(
        string key, T value, 
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            logger.LogDebug("Setting cached value for key: {Key}", key);
            
            var jsonValue = JsonSerializer.Serialize(value);
            var expiry = expiration ?? TimeSpan.FromMinutes(30);
            
            await _database.StringSetAsync(key, jsonValue, expiry);
            
            logger.LogDebug("Cached value set for key: {Key} with expiry: {Expiry}", key, expiry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting cached value for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug("Removing cached value for key: {Key}", key);
            
            await _database.KeyDeleteAsync(key);
            
            logger.LogDebug("Cached value removed for key: {Key}", key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing cached value for key: {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug("Removing cached values by pattern: {Pattern}", pattern);
            
            var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern);
            
            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
            }
            
            logger.LogDebug("Cached values removed by pattern: {Pattern}", pattern);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing cached values by pattern: {Pattern}", pattern);
        }
    }
}