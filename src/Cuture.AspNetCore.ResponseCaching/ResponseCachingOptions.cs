using System;
using System.Threading;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.Metadatas;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 执行锁定超时后的处理委托
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="filterContext"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public delegate Task ExecutionLockTimeoutFallbackDelegate(string cacheKey, FilterContext filterContext, Func<Task> next);

    /// <summary>
    /// 缓存选项
    /// </summary>
    public class ResponseCachingOptions
    {
        #region Private 字段

        private CacheStoreLocation _defaultCacheStoreLocation = CacheStoreLocation.Memory;
        private ExecutingLockMode _defaultLockMode = ExecutingLockMode.None;
        private CacheKeyStrictMode _defaultStrictMode = CacheKeyStrictMode.Ignore;
        private int _maxCacheableResponseLength = ResponseCachingConstants.DefaultMaxCacheableResponseLength;
        private int _maxCacheKeyLength = ResponseCachingConstants.DefaultMaxCacheKeyLength;
        private IMemoryCache _resultLocalCache = CreatedDefaultResultLocalCache();
        private int defaultLockMillisecondsTimeout = ResponseCachingConstants.DefaultLockMillisecondsTimeout;

        #endregion Private 字段

        #region Public 属性

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
                    throw new ArgumentOutOfRangeException(nameof(DefaultCacheStoreLocation), $"不能为 {nameof(CacheStoreLocation.Default)}");
                }
                _defaultCacheStoreLocation = value;
            }
        }

        /// <summary>
        /// 默认执行锁定模式
        /// <para/>
        /// <inheritdoc cref="IExecutingLockMetadata.LockMode"/>
        /// </summary>
        public ExecutingLockMode DefaultExecutingLockMode
        {
            get => _defaultLockMode;
            set
            {
                if (value == ExecutingLockMode.Default)
                {
                    throw new ArgumentOutOfRangeException(nameof(DefaultExecutingLockMode), $"不能为 {nameof(ExecutingLockMode.Default)}");
                }
                _defaultLockMode = value;
            }
        }

        /// <summary>
        /// 锁定执行时，默认的本地缓存可用时间（毫秒）
        /// </summary>
        public uint DefaultLocalCacheAvailableMilliseconds { get; set; } = 1500;

        /// <summary>
        /// 默认锁定等待超时时间（毫秒）<para/>
        /// 默认值为 <see cref="ResponseCachingConstants.DefaultLockMillisecondsTimeout"/><para/>
        /// 如果值为 <see cref="Timeout.Infinite"/>(-1) 则无限等待
        /// </summary>
        public int DefaultLockMillisecondsTimeout
        {
            get => defaultLockMillisecondsTimeout;
            set => defaultLockMillisecondsTimeout = Checks.ThrowIfLockMillisecondsTimeoutInvalid(value).Value;
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
                    throw new ArgumentOutOfRangeException(nameof(DefaultStrictMode), $"不能为 {nameof(CacheKeyStrictMode.Default)}");
                }
                _defaultStrictMode = value;
            }
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 锁定执行时，响应值本地缓存使用的 <see cref="IMemoryCache"/>
        /// <para/>
        /// 不设置或置空时将会使用<see cref="MemoryCache"/>构造一个，其参数为默认的<see cref="MemoryCacheOptions"/>
        /// </summary>
        public IMemoryCache LockedExecutionLocalResultCache
        {
            get => _resultLocalCache;
            set
            {
                if (value is null)
                {
                    _resultLocalCache = CreatedDefaultResultLocalCache();
                }
                else
                {
                    _resultLocalCache = value;
                }
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
                Checks.ThrowIfDumpStreamInitialCapacityTooSmall(value, nameof(MaxCacheableResponseLength));
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
                    throw new ArgumentOutOfRangeException(nameof(MaxCacheKeyLength), "不能小于 2");
                }
                _maxCacheKeyLength = value;
            }
        }

        /// <summary>
        /// 无法使用锁执行请求时（Semaphore池用尽）的回调<para/>
        /// 默认只会返回状态429
        /// </summary>
        public Func<string, FilterContext, Task>? OnCannotExecutionThroughLock { get; set; }

        /// <summary>
        /// <inheritdoc cref="ExecutionLockTimeoutFallbackDelegate"/><para/>
        /// 默认只会返回状态429
        /// </summary>
        public ExecutionLockTimeoutFallbackDelegate? OnExecutionLockTimeoutFallback { get; set; }

        #endregion Public 属性

        #region Private 方法

        private static IMemoryCache CreatedDefaultResultLocalCache()
        {
            return new MemoryCache(new MemoryCacheOptions());
        }

        #endregion Private 方法
    }
}