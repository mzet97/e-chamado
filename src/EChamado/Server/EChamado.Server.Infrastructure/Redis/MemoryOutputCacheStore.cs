using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Memory;

namespace EChamado.Server.Infrastructure.Redis;

public class MemoryOutputCacheStore : IOutputCacheStore
{
    private readonly IMemoryCache _memoryCache;

    public MemoryOutputCacheStore(IMemoryCache? memoryCache = null)
    {
        _memoryCache = memoryCache ?? new MemoryCache(new MemoryCacheOptions());
    }

    public ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken)
    {
        var result = _memoryCache.TryGetValue(key, out var value) ? (byte[]?)value : null;
        return ValueTask.FromResult(result);
    }

    public ValueTask SetAsync(string key, byte[] value, string[]? tags, TimeSpan validFor, CancellationToken cancellationToken)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = validFor,
            Size = value.Length
        };

        _memoryCache.Set(key, value, options);
        return ValueTask.CompletedTask;
    }

    public ValueTask EvictByTagAsync(string tag, CancellationToken cancellationToken)
    {
        // Memory cache doesn't support tag-based eviction easily
        // This is a limitation of using memory cache as fallback
        return ValueTask.CompletedTask;
    }
}