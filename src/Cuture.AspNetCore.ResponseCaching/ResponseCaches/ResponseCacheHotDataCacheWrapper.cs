using System;
using System.Threading.Tasks;

namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches
{
    /// <summary>
    /// 响应缓存的热数据缓存包装器
    /// </summary>
    public sealed class ResponseCacheHotDataCacheWrapper : IDistributedResponseCache, IDisposable
    {
        #region Private 字段

        private readonly IDistributedResponseCache _distributedCache;
        private readonly IHotDataCache _hotDataCache;

        #endregion Private 字段

        #region Public 构造函数

        /// <inheritdoc cref="ResponseCacheHotDataCacheWrapper"/>
        public ResponseCacheHotDataCacheWrapper(IDistributedResponseCache distributedCache, IHotDataCache hotDataCache)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _hotDataCache = hotDataCache ?? throw new ArgumentNullException(nameof(hotDataCache));
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public void Dispose()
        {
            _hotDataCache.Dispose();
        }

        /// <inheritdoc/>
        public Task<ResponseCacheEntry?> GetAsync(string key)
        {
            if (_hotDataCache.Get(key) is ResponseCacheEntry cacheEntry)
            {
                return Task.FromResult(cacheEntry)!;
            }
            return _distributedCache.GetAsync(key);
        }

        /// <inheritdoc/>
        public Task SetAsync(string key, ResponseCacheEntry entry, int duration)
        {
            _hotDataCache.Set(key, entry, duration);
            return _distributedCache.SetAsync(key, entry, duration);
        }

        #endregion Public 方法
    }
}