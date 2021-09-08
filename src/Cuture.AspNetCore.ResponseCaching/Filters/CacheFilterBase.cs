using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.Diagnostics;
using Cuture.AspNetCore.ResponseCaching.Interceptors;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Cuture.AspNetCore.ResponseCaching.Filters
{
    /// <summary>
    /// CacheFilter基类
    /// </summary>
    public abstract class CacheFilterBase<TFilterExecutingContext> : IDisposable where TFilterExecutingContext : FilterContext
    {
        #region Private 字段

        private readonly CachingDiagnosticsAccessor _cachingDiagnosticsAccessor;
        private readonly OnCacheStoringDelegate<ActionContext> _onCacheStoringDelegate;
        private bool _disposedValue;

        #endregion Private 字段

        #region Protected 属性

        /// <inheritdoc cref="Diagnostics.CachingDiagnostics"/>
        protected CachingDiagnostics CachingDiagnostics { get; }

        /// <summary>
        /// ResponseCaching上下文
        /// </summary>
        protected ResponseCachingContext Context { get; }

        /// <inheritdoc cref="ILogger"/>
        protected ILogger Logger { get; }

        /// <summary>
        /// 响应缓存容器
        /// </summary>
        protected IResponseCache ResponseCache { get; }

        #endregion Protected 属性

        #region Public 构造函数

        /// <summary>
        /// CacheFilter基类
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cachingDiagnosticsAccessor"></param>
        public CacheFilterBase(ResponseCachingContext context, CachingDiagnosticsAccessor cachingDiagnosticsAccessor)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _cachingDiagnosticsAccessor = cachingDiagnosticsAccessor;
            CachingDiagnostics = _cachingDiagnosticsAccessor.CachingDiagnostics;
            Logger = CachingDiagnostics.Logger;
            ResponseCache = Context.ResponseCache;
            _onCacheStoringDelegate = InternalStoreCacheAsync;
        }

        #endregion Public 构造函数

        #region Protected 方法

        #region StoreCache

        /// <summary>
        /// 检查并保存缓存
        /// </summary>
        /// <param name="executingContext"></param>
        /// <param name="executedContext"></param>
        /// <param name="key"></param>
        /// <param name="cacheEntry"></param>
        /// <returns></returns>
        protected async Task<ResponseCacheEntry?> CheckAndStoreCacheAsync(ResourceExecutingContext executingContext,
                                                                          ResourceExecutedContext executedContext,
                                                                          string key,
                                                                          ResponseCacheEntry cacheEntry)
        {
            if (Context.CacheDeterminer.CanCaching(executedContext, cacheEntry))
            {
                if (cacheEntry.Body.Length <= Context.MaxCacheableResponseLength)
                {
                    return await Context.Interceptors.OnCacheStoringAsync(executingContext, key, cacheEntry, StoreCacheAsync);
                }
                else
                {
                    CachingDiagnostics.CacheBodyTooLarge(key, cacheEntry.Body, Context.MaxCacheableResponseLength, executingContext, Context);
                }
            }

            return null;
        }

        /// <summary>
        /// 保存响应
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="key"></param>
        /// <param name="cacheEntry"></param>
        /// <returns>进行内存缓存，以用于立即响应的 <see cref="ResponseCacheEntry"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Task<ResponseCacheEntry?> StoreCacheAsync(ActionContext actionContext, string key, ResponseCacheEntry cacheEntry)
        {
            return Context.Interceptors.OnCacheStoringAsync(actionContext, key, cacheEntry, _onCacheStoringDelegate);
        }

        private async Task<ResponseCacheEntry?> InternalStoreCacheAsync(ActionContext actionContext, string key, ResponseCacheEntry cacheEntry)
        {
            await ResponseCache.SetAsync(key, cacheEntry);
            return cacheEntry;
        }

        #endregion StoreCache

        #region Response with cache

        /// <summary>
        /// 尝试以缓存处理请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected async Task<bool> TryServeFromCacheAsync(TFilterExecutingContext context, string key)
        {
            var cacheEntry = await ResponseCache.GetAsync(key);
            if (cacheEntry != null)
            {
                return await WriteCacheToResponseWithInterceptorAsync(context, cacheEntry);
            }

            return false;
        }

        /// <summary>
        /// 将缓存写入响应
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cacheEntry"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected async Task<bool> WriteCacheToResponseAsync(ActionContext context, ResponseCacheEntry cacheEntry)
        {
            CachingDiagnostics.ResponseFromCache(context, cacheEntry, Context);
            context.HttpContext.Response.ContentType = cacheEntry.ContentType;
            await context.HttpContext.Response.BodyWriter.WriteAsync(cacheEntry.Body, context.HttpContext.RequestAborted);
            return true;
        }

        /// <summary>
        /// 将缓存写入响应，经过拦截器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cacheEntry"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Task<bool> WriteCacheToResponseWithInterceptorAsync(ActionContext context, ResponseCacheEntry cacheEntry)
        {
            return Context.Interceptors.OnResponseWritingAsync(context, cacheEntry, WriteCacheToResponseAsync);
        }

        #endregion Response with cache

        #endregion Protected 方法

        #region Dispose

        /// <summary>
        ///
        /// </summary>
        ~CacheFilterBase()
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
                Context.Dispose();
                _disposedValue = true;
            }
        }

        #endregion Dispose
    }
}