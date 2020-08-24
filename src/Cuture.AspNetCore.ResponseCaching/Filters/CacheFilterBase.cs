using System;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Cuture.AspNetCore.ResponseCaching.Filters
{
    /// <summary>
    /// CacheFilter基类
    /// </summary>
    public abstract class CacheFilterBase<TFilterExecutingContext, TLocalCachingData> where TFilterExecutingContext : FilterContext
    {
        /// <summary>
        /// ResponseCaching上下文
        /// </summary>
        protected ResponseCachingContext<TFilterExecutingContext, TLocalCachingData> Context { get; }

        /// <summary>
        /// 响应缓存容器
        /// </summary>
        protected IResponseCache ResponseCache => Context.ResponseCache;

        /// <inheritdoc cref="ILogger"/>
        protected ILogger Logger { get; }

        /// <summary>
        /// CacheFilter基类
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public CacheFilterBase(ResponseCachingContext<TFilterExecutingContext, TLocalCachingData> context, ILogger logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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
                await WriteCacheToResponseAsync(context, cacheEntry);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 将缓存写入响应
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cacheEntry"></param>
        /// <returns></returns>
        protected static async Task WriteCacheToResponseAsync(FilterContext context, ResponseCacheEntry cacheEntry)
        {
            context.HttpContext.Response.ContentType = cacheEntry.ContentType;
            await context.HttpContext.Response.BodyWriter.WriteAsync(cacheEntry.Body, context.HttpContext.RequestAborted);
        }
    }
}