namespace BookLibraryAPI.Core.Domain.Interfaces.Ports.Caching;

public interface ICachePort
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, 
        CancellationToken cancellationToken = default) where T : class;
    
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
}