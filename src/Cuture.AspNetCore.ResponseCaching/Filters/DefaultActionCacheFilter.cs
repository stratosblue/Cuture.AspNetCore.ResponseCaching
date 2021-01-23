using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

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
        /// <param name="logger"></param>
        public DefaultActionCacheFilter(ResponseCachingContext<ActionExecutingContext, IActionResult> context, ILogger logger) : base(context, logger)
        {
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var key = (await Context.KeyGenerator.GenerateKeyAsync(context)).ToLowerInvariant();
            Debug.WriteLine(key);

            if (key.Length > Context.MaxCacheKeyLength)
            {
                Logger.LogWarning("CacheKey is too long to cache. maxLength: {0} key: {1}", Context.MaxCacheKeyLength, key);
                await next();
                return;
            }

            if (string.IsNullOrEmpty(key))
            {
                await next();
                return;
            }

            if (await TryServeFromCacheAsync(context, key))
            {
                return;
            }

            await ExecutingRequestAsync(context, next, key);
        }

        /// <inheritdoc/>
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            await next();

            var httpItems = context.HttpContext.Items;
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
                        var cacheEntry = new ResponseCacheEntry(context.HttpContext.Response.ContentType, responseBody);

                        await Context.Interceptors.OnCacheStoringAsync(context, key, cacheEntry, Context.Duration, SetCacheAsync);
                    }
                    else
                    {
                        Logger.LogWarning("Response too long to cache, key: {0}, maxLength: {1}, length: {2}", key, Context.MaxCacheableResponseLength, responseBody.Length);
                    }
                }
                finally
                {
                    context.HttpContext.Response.Body = originalBody;
                    dumpStream.Dispose();
                }
            }
        }

        #endregion Public 方法

        #region Protected 方法

        /// <summary>
        /// 执行请求并替换响应流
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected async Task<IActionResult?> ExecutingAndReplaceResponseStreamAsync(ActionExecutingContext context, ActionExecutionDelegate next, string key)
        {
            var response = context.HttpContext.Response;
            var originalBody = response.Body;
            var dumpStream = Context.DumpStreamFactory.Create();

            try
            {
                response.Body = dumpStream;
                var executedContext = await next();

                if (Context.CacheDeterminer.CanCaching(executedContext))
                {
                    context.HttpContext.Items.Add(ResponseCachingConstants.ResponseCachingCacheKeyKey, key);
                    context.HttpContext.Items.Add(ResponseCachingConstants.ResponseCachingDumpStreamKey, dumpStream);
                    context.HttpContext.Items.Add(ResponseCachingConstants.ResponseCachingOriginalStreamKey, originalBody);

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
                await Context.ExecutingLocker.ProcessCacheWithLockAsync(key,
                                                                        context,
                                                                        inActionResult => SetResultToContextWithInterceptorAsync(context, inActionResult),
                                                                        () => ExecutingAndReplaceResponseStreamAsync(context, next, key));
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
        private static Task SetResultToContextAsync(ActionExecutingContext context, IActionResult actionResult)
        {
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