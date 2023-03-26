using System;

using Microsoft.Extensions.Caching.Memory;

namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches;

/// <summary>
/// LRU算法的热点数据缓存
/// </summary>
public class LRUHotDataCache : IHotDataCache
{
    #region Private 字段

    private readonly IBoundedMemoryCache<string, ResponseCacheEntry> _boundedMemoryCache;

    #endregion Private 字段

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="LRUHotDataCache"/>
    /// </summary>
    /// <param name="capacity">容量</param>
    public LRUHotDataCache(int capacity)
    {
        _boundedMemoryCache = BoundedMemoryCache.CreateLRU<string, ResponseCacheEntry>(capacity);
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public ResponseCacheEntry? Get(string key)
    {
        if (_boundedMemoryCache.TryGet(key, out var result))
        {
            if (result!.IsExpired())
            {
                _boundedMemoryCache.Remove(key);
                return null;
            }
            return result;
        }
        return null;
    }

    /// <inheritdoc/>
    public void Set(string key, ResponseCacheEntry entry)
    {
        _boundedMemoryCache.Add(key, entry);
    }

    #endregion Public 方法
}