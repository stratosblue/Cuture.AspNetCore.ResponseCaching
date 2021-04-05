using System;

namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches
{
    /// <summary>
    /// 热点缓存项包装器
    /// </summary>
    public class HotResponseCacheEntryWrapper
    {
        #region Public 属性

        /// <summary>
        /// 缓存项
        /// </summary>
        public ResponseCacheEntry CacheEntry { get; }

        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime Expiration { get; set; }

        #endregion Public 属性

        #region Public 构造函数

        /// <inheritdoc cref="HotResponseCacheEntryWrapper"/>
        public HotResponseCacheEntryWrapper(ResponseCacheEntry cacheEntry, DateTime expiration)
        {
            CacheEntry = cacheEntry;
            Expiration = expiration;
        }

        #endregion Public 构造函数
    }
}