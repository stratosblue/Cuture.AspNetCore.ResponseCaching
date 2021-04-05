using System;

using Microsoft.Extensions.Caching.Memory;

namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches
{
    /// <summary>
    /// LRU算法的热点数据缓存
    /// </summary>
    public class LRUHotDataCache : IHotDataCache
    {
        #region Private 字段

        private readonly IBoundedMemoryCache<string, HotResponseCacheEntryWrapper> _boundedMemoryCache;

        #endregion Private 字段

        #region Public 构造函数

        /// <summary>
        /// <inheritdoc cref="LRUHotDataCache"/>
        /// </summary>
        /// <param name="capacity">容量</param>
        public LRUHotDataCache(int capacity)
        {
            _boundedMemoryCache = BoundedMemoryCache.CreateLRU<string, HotResponseCacheEntryWrapper>(capacity);
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        /// <inheritdoc/>
        public ResponseCacheEntry? Get(string key)
        {
            if (_boundedMemoryCache.TryGet(key, out var result))
            {
                if (result!.Expiration < DateTime.UtcNow)
                {
                    _boundedMemoryCache.Remove(key);
                    return null;
                }
                return result.CacheEntry;
            }
            return null;
        }

        /// <inheritdoc/>
        public void Set(string key, ResponseCacheEntry entry, int duration)
        {
            _boundedMemoryCache.Add(key, new HotResponseCacheEntryWrapper(entry, DateTime.UtcNow.AddSeconds(duration)));
        }

        #endregion Public 方法
    }
}