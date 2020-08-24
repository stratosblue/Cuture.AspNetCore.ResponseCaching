using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches
{
    /// <summary>
    /// 默认的基于内存的响应缓存
    /// </summary>
    public sealed class DefaultMemoryResponseCache : IMemoryResponseCache, IDisposable
    {
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// 默认的基于内存的响应缓存
        /// </summary>
        /// <param name="loggerFactory"></param>
        public DefaultMemoryResponseCache(ILoggerFactory loggerFactory)
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions(), loggerFactory);
        }

        /// <inheritdoc/>
        public Task<ResponseCacheEntry> GetAsync(string key)
        {
            _memoryCache.TryGetValue<ResponseCacheEntry>(key, out var cacheEntry);
            return Task.FromResult(cacheEntry);
        }

        /// <inheritdoc/>
        public Task SetAsync(string key, ResponseCacheEntry entry, int duration)
        {
            _memoryCache.Set(key, entry, TimeSpan.FromSeconds(duration));
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _memoryCache.Dispose();
        }
    }
}