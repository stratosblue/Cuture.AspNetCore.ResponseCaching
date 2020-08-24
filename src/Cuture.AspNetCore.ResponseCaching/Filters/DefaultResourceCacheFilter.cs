using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Cuture.AspNetCore.ResponseCaching.Filters
{
    /// <summary>
    /// 默认的基于IAsyncResourceFilter的缓存过滤Filter
    /// </summary>
    public class DefaultResourceCacheFilter : CacheFilterBase<ResourceExecutingContext, ResponseCacheEntry>, IAsyncResourceFilter
    {
        /// <summary>
        /// 默认的基于ResourceFilter的缓存过滤Filter
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public DefaultResourceCacheFilter(ResponseCachingContext<ResourceExecutingContext, ResponseCacheEntry> context, ILogger logger) : base(context, logger)
        {
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
                await Context.ExecutingLocker.ProcessCacheWithLockAsync(key,
                                                                        context,
                                                                        inCacheEntry => WriteCacheToResponseAsync(context, inCacheEntry),
                                                                        () => DumpAndCacheResponseAsync(context, next, key));
            }
            else
            {
                await DumpAndCacheResponseAsync(context, next, key);
            }
        }

        /// <summary>
        /// 转储并缓存响应
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected async Task<ResponseCacheEntry> DumpAndCacheResponseAsync(ResourceExecutingContext context, ResourceExecutionDelegate next, string key)
        {
            var response = context.HttpContext.Response;
            var originalBody = response.Body;
            using var dumpStream = Context.DumpStreamFactory.Create();

            ResponseCacheEntry cacheEntry = null;
            ResourceExecutedContext executedContext = null;
            try
            {
                response.Body = dumpStream;
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
                    _ = ResponseCache.SetAsync(key, cacheEntry, Context.Duration);
                }
                else
                {
                    Logger.LogWarning("Response too long to cache, key: {0}, maxLength: {1}, length: {2}", key, Context.MaxCacheableResponseLength, cacheEntry.Body.Length);
                }
                return cacheEntry;
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
#pragma warning disable CA1308 // 将字符串规范化为大写
            var key = (await Context.KeyGenerator.GenerateKeyAsync(context)).ToLowerInvariant();
#pragma warning restore CA1308 // 将字符串规范化为大写

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
    }
}