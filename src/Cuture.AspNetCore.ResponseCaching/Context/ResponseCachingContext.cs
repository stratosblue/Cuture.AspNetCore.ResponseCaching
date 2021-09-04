using System;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;
using Cuture.AspNetCore.ResponseCaching.Interceptors;
using Cuture.AspNetCore.ResponseCaching.Internal;
using Cuture.AspNetCore.ResponseCaching.Lockers;
using Cuture.AspNetCore.ResponseCaching.Metadatas;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 响应缓存上下文
    /// </summary>
    /// <typeparam name="TFilterContext">FilterContext</typeparam>
    /// <typeparam name="TLocalCachingData">本地缓存类型</typeparam>
    public class ResponseCachingContext<TFilterContext, TLocalCachingData> : IDisposable where TFilterContext : FilterContext
    {
        #region Private 字段

        private bool _disposedValue;

        #endregion Private 字段

        #region Public 属性

        /// <summary>
        /// 响应缓存确定器
        /// </summary>
        public IResponseCacheDeterminer CacheDeterminer { get; set; }

        /// <summary>
        /// 用于Dump的MemeoryStream初始化大小
        /// </summary>
        public int DumpStreamCapacity { get; }

        /// <summary>
        /// 缓存有效时长（秒）
        /// </summary>
        public int Duration { get; }

        /// <summary>
        /// 执行锁定器
        /// </summary>
        public IRequestExecutingLocker<TFilterContext, TLocalCachingData> ExecutingLocker { get; }

        /// <summary>
        /// 拦截器集合
        /// </summary>
        public InterceptorAggregator Interceptors { get; }

        /// <summary>
        /// 缓存Key生成器
        /// </summary>
        public ICacheKeyGenerator KeyGenerator { get; }

        /// <summary>
        /// 最大可缓存响应长度
        /// </summary>
        public int MaxCacheableResponseLength { get; } = -1;

        /// <summary>
        /// 缓存Key的最大长度
        /// </summary>
        public int MaxCacheKeyLength { get; }

        /// <summary>
        /// 无法使用锁执行请求时（Semaphore池用尽）的回调
        /// </summary>
        public Func<string, FilterContext, Task> OnCannotExecutionThroughLock { get; }

        /// <summary>
        /// 响应缓存容器
        /// </summary>
        public IResponseCache ResponseCache { get; }

        #endregion Public 属性

        #region Protected 构造函数

        /// <inheritdoc cref="ResponseCachingContext{TFilterContext, TLocalCachingData}"/>
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        protected ResponseCachingContext()
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        {
        }

        #endregion Protected 构造函数

        #region Public 构造函数

        /// <summary>
        /// <inheritdoc cref="ResponseCachingContext{TFilterContext, TLocalCachingData}"/>
        /// </summary>
        /// <param name="metadatas">Action端点的 <see cref="EndpointMetadataCollection"/></param>
        /// <param name="cacheKeyGenerator"></param>
        /// <param name="executingLocker"></param>
        /// <param name="responseCache"></param>
        /// <param name="cacheDeterminer"></param>
        /// <param name="options"></param>
        /// <param name="interceptorAggregator"></param>
        /// <param name="dumpStreamCapacity"></param>
        public ResponseCachingContext(EndpointMetadataCollection metadatas,
                                      ICacheKeyGenerator cacheKeyGenerator,
                                      IRequestExecutingLocker<TFilterContext, TLocalCachingData> executingLocker,
                                      IResponseCache responseCache,
                                      IResponseCacheDeterminer cacheDeterminer,
                                      ResponseCachingOptions options,
                                      InterceptorAggregator interceptorAggregator,
                                      int dumpStreamCapacity)
        {
            if (metadatas is null)
            {
                throw new ArgumentNullException(nameof(metadatas));
            }

            MaxCacheableResponseLength = Checks.ThrowIfMaxCacheableResponseLengthTooSmall(Metadata<IMaxCacheableResponseLengthMetadata>()?.MaxCacheableResponseLength ?? options.MaxCacheableResponseLength);

            MaxCacheKeyLength = options.MaxCacheKeyLength;

            KeyGenerator = cacheKeyGenerator ?? throw new ArgumentNullException(nameof(cacheKeyGenerator));
            ExecutingLocker = executingLocker;
            ResponseCache = responseCache ?? throw new ArgumentNullException(nameof(responseCache));
            CacheDeterminer = cacheDeterminer ?? throw new ArgumentNullException(nameof(cacheDeterminer));
            OnCannotExecutionThroughLock = options.OnCannotExecutionThroughLock ?? DefaultCannotExecutionThroughLockCallback.SetStatus429;
            Duration = Checks.ThrowIfDurationTooSmall(RequiredMetadata<IResponseDurationMetadata>().Duration);

            Interceptors = interceptorAggregator;
            DumpStreamCapacity = Checks.ThrowIfDumpStreamInitialCapacityTooSmall(dumpStreamCapacity, nameof(dumpStreamCapacity));

            TMetadata? Metadata<TMetadata>() where TMetadata : class => metadatas.GetMetadata<TMetadata>();
            TMetadata RequiredMetadata<TMetadata>() where TMetadata : class => metadatas.RequiredMetadata<TMetadata>();
        }

        #endregion Public 构造函数

        #region Dispose

        /// <summary>
        ///
        /// </summary>
        ~ResponseCachingContext()
        {
            Dispose(disposing: false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (ExecutingLocker is not null
                    && !ExecutingLocker.IsShared)
                {
                    ExecutingLocker.Dispose();
                }
                _disposedValue = true;
            }
        }

        #endregion Dispose
    }
}