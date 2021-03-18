using System;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.Diagnostics;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Filters
{
    /// <summary>
    /// 默认的基于IAsyncResourceFilter的缓存过滤Filter
    /// </summary>
    public class DefaultResourceCacheFilter : CacheFilterBase<ResourceExecutingContext, ResponseCacheEntry>, IAsyncResourceFilter
    {
        #region Public 构造函数

        /// <summary>
        /// 默认的基于ResourceFilter的缓存过滤Filter
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cachingDiagnosticsAccessor"></param>
        public DefaultResourceCacheFilter(ResponseCachingContext<ResourceExecutingContext, ResponseCacheEntry> context,
                                          CachingDiagnosticsAccessor cachingDiagnosticsAccessor) : base(context, cachingDiagnosticsAccessor)
        {
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (CachingDiagnostics is null)
            {
                return InternalOnResourceExecutionAsync(context, next);
            }
            return InternalOnResourceExecutionWithDiagnosticAsync(context, next, CachingDiagnostics);
        }

        #endregion Public 方法

        #region OnResourceExecutionAsync

        private async Task InternalOnResourceExecutionAsync(ResourceExecutingContext executingContext, ResourceExecutionDelegate next)
        {
            var key = (await Context.KeyGenerator.GenerateKeyAsync(executingContext)).ToLowerInvariant();

            if (key.Length > Context.MaxCacheKeyLength)
            {
                CachingDiagnostics.CacheKeyTooLong(key, Context.MaxCacheKeyLength, executingContext, Context);
                await next();
                return;
            }

            if (string.IsNullOrEmpty(key))
            {
                CachingDiagnostics.NoCachingFounded(key, executingContext, Context);
                await next();
                return;
            }

            if (await TryServeFromCacheAsync(executingContext, key))
            {
                return;
            }

            await ExecutingRequestAsync(executingContext, next, key);
        }

        private async Task InternalOnResourceExecutionWithDiagnosticAsync(ResourceExecutingContext executingContext, ResourceExecutionDelegate next, CachingDiagnostics cachingDiagnostics)
        {
            cachingDiagnostics.StartProcessingCache(executingContext, Context);
            try
            {
                var key = (await Context.KeyGenerator.GenerateKeyAsync(executingContext)).ToLowerInvariant();

                cachingDiagnostics.CacheKeyGenerated(executingContext, key, Context.KeyGenerator, Context);

                if (key.Length > Context.MaxCacheKeyLength)
                {
                    cachingDiagnostics.CacheKeyTooLong(key, Context.MaxCacheKeyLength, executingContext, Context);
                    await next();
                    return;
                }

                if (string.IsNullOrEmpty(key))
                {
                    await next();
                    return;
                }

                if (await TryServeFromCacheAsync(executingContext, key))
                {
                    return;
                }

                await ExecutingRequestAsync(executingContext, next, key);
            }
            finally
            {
                cachingDiagnostics.EndProcessingCache(executingContext, Context);
            }
        }

        #endregion OnResourceExecutionAsync

        #region Protected 方法

        /// <summary>
        /// 转储并缓存响应
        /// </summary>
        /// <param name="executingContext"></param>
        /// <param name="next"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected async Task<ResponseCacheEntry?> DumpAndCacheResponseAsync(ResourceExecutingContext executingContext, ResourceExecutionDelegate next, string key)
        {
            var response = executingContext.HttpContext.Response;
            var originalBody = response.Body;
            using var dumpStream = Context.DumpStreamFactory.Create();

            ResponseCacheEntry? cacheEntry = null;
            ResourceExecutedContext executedContext;
            try
            {
                response.Body = dumpStream;

                CachingDiagnostics.NoCachingFounded(key, executingContext, Context);

                executedContext = await next();
                dumpStream.Position = 0;
                await dumpStream.CopyToAsync(originalBody);

                cacheEntry = new ResponseCacheEntry(response.ContentType, dumpStream.ToArray().AsMemory());
            }
            finally
            {
                response.Body = originalBody;
            }

            if (Context.CacheDeterminer.CanCaching(executedContext, cacheEntry))
            {
                if (cacheEntry.Body.Length <= Context.MaxCacheableResponseLength)
                {
                    await Context.Interceptors.OnCacheStoringAsync(executingContext, key, cacheEntry, Context.Duration, SetCacheAsync);
                }
                else
                {
                    CachingDiagnostics.CacheBodyTooLarge(key, cacheEntry.Body, Context.MaxCacheableResponseLength, executingContext, Context);
                }
                return cacheEntry;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected async Task ExecutingRequestAsync(ResourceExecutingContext context, ResourceExecutionDelegate next, string key)
        {
            if (Context.ExecutingLocker != null)
            {
                var executed = await Context.ExecutingLocker.ProcessCacheWithLockAsync(key,
                                                                                       context,
                                                                                       inCacheEntry => WriteCacheToResponseWithInterceptorAsync(context, inCacheEntry),
                                                                                       () => DumpAndCacheResponseAsync(context, next, key));
                if (!executed)
                {
                    CachingDiagnostics.CannotExecutionThroughLock(key, context, Context);

                    await Context.OnCannotExecutionThroughLock(key, context);
                    return;
                }
            }
            else
            {
                await DumpAndCacheResponseAsync(context, next, key);
            }
        }

        #endregion Protected 方法
    }
}