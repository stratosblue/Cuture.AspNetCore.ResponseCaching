using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.Diagnostics;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Filters
{
    /// <summary>
    /// 默认的基于ActionFilter的缓存过滤Filter
    /// </summary>
    public class DefaultActionCacheFilter : CacheFilterBase<ActionExecutingContext, IActionResult>, IAsyncActionFilter, IAsyncResourceFilter
    {
        #region Public 构造函数

        /// <summary>
        /// 默认的基于ActionFilter的缓存过滤Filter
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cachingDiagnosticsAccessor"></param>
        public DefaultActionCacheFilter(ResponseCachingContext<ActionExecutingContext, IActionResult> context, CachingDiagnosticsAccessor cachingDiagnosticsAccessor) : base(context, cachingDiagnosticsAccessor)
        {
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public async Task OnActionExecutionAsync(ActionExecutingContext executingContext, ActionExecutionDelegate next)
        {
            var key = (await Context.KeyGenerator.GenerateKeyAsync(executingContext)).ToLowerInvariant();
            CachingDiagnostics.CacheKeyGenerated(executingContext, key, Context.KeyGenerator, Context);

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

        /// <inheritdoc/>
        public async Task OnResourceExecutionAsync(ResourceExecutingContext executingContext, ResourceExecutionDelegate next)
        {
            CachingDiagnostics.StartProcessingCache(executingContext, Context);
            try
            {
                await next();

                var httpItems = executingContext.HttpContext.Items;
                if (httpItems.TryGetValue(ResponseCachingConstants.ResponseCachingOriginalStreamKey, out var savedOriginalBody)
                    && savedOriginalBody is Stream originalBody
                    && httpItems.TryGetValue(ResponseCachingConstants.ResponseCachingDumpStreamKey, out var savedDumpStream)
                    && savedDumpStream is MemoryStream dumpStream
                    && httpItems.TryGetValue(ResponseCachingConstants.ResponseCachingCacheKeyKey, out var savedKey)
                    && savedKey is string key)
                {
                    try
                    {
                        dumpStream.Position = 0;
                        await dumpStream.CopyToAsync(originalBody);

                        var responseBody = dumpStream.ToArray().AsMemory();

                        if (responseBody.Length <= Context.MaxCacheableResponseLength)
                        {
                            var cacheEntry = new ResponseCacheEntry(executingContext.HttpContext.Response.ContentType, responseBody);

                            await Context.Interceptors.OnCacheStoringAsync(executingContext, key, cacheEntry, Context.Duration, SetCacheAsync);
                        }
                        else
                        {
                            CachingDiagnostics.CacheBodyTooLarge(key, responseBody, Context.MaxCacheableResponseLength, executingContext, Context);
                        }
                    }
                    finally
                    {
                        executingContext.HttpContext.Response.Body = originalBody;
                        dumpStream.Dispose();
                    }
                }
            }
            finally
            {
                CachingDiagnostics.EndProcessingCache(executingContext, Context);
            }
        }

        #endregion Public 方法

        #region Protected 方法

        /// <summary>
        /// 执行请求并替换响应流
        /// </summary>
        /// <param name="executingContext"></param>
        /// <param name="next"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected async Task<IActionResult?> ExecutingAndReplaceResponseStreamAsync(ActionExecutingContext executingContext, ActionExecutionDelegate next, string key)
        {
            var response = executingContext.HttpContext.Response;
            var originalBody = response.Body;
            var dumpStream = Context.DumpStreamFactory.Create();

            try
            {
                response.Body = dumpStream;

                CachingDiagnostics.NoCachingFounded(key, executingContext, Context);

                var executedContext = await next();

                if (Context.CacheDeterminer.CanCaching(executedContext))
                {
                    executingContext.HttpContext.Items.Add(ResponseCachingConstants.ResponseCachingCacheKeyKey, key);
                    executingContext.HttpContext.Items.Add(ResponseCachingConstants.ResponseCachingDumpStreamKey, dumpStream);
                    executingContext.HttpContext.Items.Add(ResponseCachingConstants.ResponseCachingOriginalStreamKey, originalBody);

                    return executedContext.Result;
                }
                response.Body = originalBody;
                dumpStream.Dispose();

                return null;
            }
            catch
            {
                //TODO 在此处还原流是否会有问题？
                response.Body = originalBody;
                dumpStream.Dispose();

                throw;
            }
        }

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected async Task ExecutingRequestAsync(ActionExecutingContext context, ActionExecutionDelegate next, string key)
        {
            if (Context.ExecutingLocker != null)
            {
                var executed = await Context.ExecutingLocker.ProcessCacheWithLockAsync(key,
                                                                                       context,
                                                                                       inActionResult => SetResultToContextWithInterceptorAsync(context, inActionResult),
                                                                                       () => ExecutingAndReplaceResponseStreamAsync(context, next, key));
                if (!executed)
                {
                    CachingDiagnostics.CannotExecutionThroughLock(key, context, Context);

                    await Context.OnCannotExecutionThroughLock(key, context);
                    return;
                }
            }
            else
            {
                await ExecutingAndReplaceResponseStreamAsync(context, next, key);
            }
        }

        #endregion Protected 方法

        #region Private 方法

        /// <summary>
        /// 将返回值设置到执行上下文
        /// </summary>
        /// <param name="context"></param>
        /// <param name="actionResult"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Task SetResultToContextAsync(ActionExecutingContext context, IActionResult actionResult)
        {
            CachingDiagnostics.ResponseFromActionResult(context, actionResult, Context);
            context.Result = actionResult;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 将返回值设置到执行上下文
        /// </summary>
        /// <param name="context"></param>
        /// <param name="actionResult"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Task SetResultToContextWithInterceptorAsync(ActionExecutingContext context, IActionResult actionResult)
        {
            return Context.Interceptors.OnResultSettingAsync(context, actionResult, SetResultToContextAsync);
        }

        #endregion Private 方法
    }
}