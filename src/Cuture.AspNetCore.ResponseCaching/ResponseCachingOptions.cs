using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 缓存选项
    /// </summary>
    public class ResponseCachingOptions : IOptions<ResponseCachingOptions>
    {
        private CacheStoreLocation _defaultCacheStoreLocation = CacheStoreLocation.Memory;
        private CacheKeyStrictMode _defaultStrictMode = CacheKeyStrictMode.Ignore;
        private ExecutingLockMode _defaultLockMode = ExecutingLockMode.None;
        private int _maxCacheableResponseLength = ResponseCachingConstants.DefaultMaxCacheableResponseLength;
        private int _maxCacheKeyLength = ResponseCachingConstants.DefaultMaxCacheKeyLength;

        /// <summary>
        /// 默认缓存存放位置
        /// </summary>
        public CacheStoreLocation DefaultCacheStoreLocation
        {
            get => _defaultCacheStoreLocation;
            set
            {
                if (value == CacheStoreLocation.Default)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(DefaultCacheStoreLocation)} 不能为 {nameof(CacheStoreLocation.Default)}");
                }
                _defaultCacheStoreLocation = value;
            }
        }

        /// <summary>
        /// 默认缓存键的严格模式
        /// </summary>
        public CacheKeyStrictMode DefaultStrictMode
        {
            get => _defaultStrictMode;
            set
            {
                if (value == CacheKeyStrictMode.Default)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(DefaultStrictMode)} 不能为 {nameof(CacheKeyStrictMode.Default)}");
                }
                _defaultStrictMode = value;
            }
        }

        /// <summary>
        /// 默认执行锁定模式
        /// <para/>
        /// <inheritdoc cref="ResponseCachingAttribute.LockMode"/>
        /// </summary>
        public ExecutingLockMode DefaultExecutingLockMode
        {
            get => _defaultLockMode;
            set
            {
                if (value == ExecutingLockMode.Default)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(DefaultExecutingLockMode)} 不能为 {nameof(ExecutingLockMode.Default)}");
                }
                _defaultLockMode = value;
            }
        }

        /// <summary>
        /// 默认的最大可缓存响应长度
        /// </summary>
        public int MaxCacheableResponseLength
        {
            get => _maxCacheableResponseLength;
            set
            {
                if (value < ResponseCachingConstants.DefaultMinMaxCacheableResponseLength)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MaxCacheableResponseLength)} 不能小于 {ResponseCachingConstants.DefaultMinMaxCacheableResponseLength}");
                }
                _maxCacheableResponseLength = value;
            }
        }

        /// <summary>
        /// 缓存Key的最大长度
        /// </summary>
        public int MaxCacheKeyLength
        {
            get => _maxCacheKeyLength;
            set
            {
                if (value < 2)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MaxCacheKeyLength)} 不能小于 2");
                }
                _maxCacheKeyLength = value;
            }
        }

        /// <inheritdoc/>
        public ResponseCachingOptions Value => this;
    }
}