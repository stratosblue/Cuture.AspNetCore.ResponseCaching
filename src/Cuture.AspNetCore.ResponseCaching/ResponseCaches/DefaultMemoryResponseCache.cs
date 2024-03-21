using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches;

/// <summary>
/// 默认的基于内存的响应缓存
/// </summary>
public sealed class DefaultMemoryResponseCache : IMemoryResponseCache, IDisposable
{
    #region Private 字段

    private readonly MemoryCache _memoryCache;

    #endregion Private 字段

    #region Public 构造函数

    /// <summary>
    /// 默认的基于内存的响应缓存
    /// </summary>
    /// <param name="loggerFactory"></param>
    public DefaultMemoryResponseCache(ILoggerFactory loggerFactory)
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions(), loggerFactory);
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public void Dispose()
    {
        _memoryCache.Dispose();
    }

    /// <inheritdoc/>
    public Task<ResponseCacheEntry?> GetAsync(string key, CancellationToken cancellationToken)
    {
        _memoryCache.TryGetValue<ResponseCacheEntry>(key, out var cacheEntry);
        return Task.FromResult(cacheEntry)!;
    }

    /// <inheritdoc/>
    public Task<bool?> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        return Task.FromResult<bool?>(null);
    }

    /// <inheritdoc/>
    public Task SetAsync(string key, ResponseCacheEntry entry, CancellationToken cancellationToken)
    {
        _memoryCache.Set(key, entry, entry.GetAbsoluteExpirationDateTimeOffset());
        return Task.CompletedTask;
    }

    #endregion Public 方法
}
