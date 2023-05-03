using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.Diagnostics;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Cuture.AspNetCore.ResponseCaching.Filters;

/// <summary>
/// 默认的基于IAsyncResourceFilter的缓存过滤Filter
/// </summary>
public class DefaultResourceCacheFilter : CacheFilterBase<ResourceExecutingContext>, IAsyncResourceFilter
{
    #region Public 构造函数

    /// <summary>
    /// 默认的基于ResourceFilter的缓存过滤Filter
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cachingDiagnosticsAccessor"></param>
    public DefaultResourceCacheFilter(ResponseCachingContext context,
                                      CachingDiagnosticsAccessor cachingDiagnosticsAccessor)
        : base(context, cachingDiagnosticsAccessor)
    {
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        return InternalOnResourceExecutionAsync(context, next, CachingDiagnostics);
    }

    #endregion Public 方法

    #region OnResourceExecutionAsync

    private async Task InternalOnResourceExecutionAsync(ResourceExecutingContext executingContext, ResourceExecutionDelegate next, CachingDiagnostics cachingDiagnostics)
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
                cachingDiagnostics.NoCachingFounded(key, executingContext, Context);
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
        using var dumpStream = executingContext.HttpContext.RequestServices.GetRequiredService<IResponseDumpStreamFactory>().Create(Context.DumpStreamCapacity);

        ResponseCacheEntry cacheEntry;
        ResourceExecutedContext executedContext;
        try
        {
            response.Body = dumpStream;

            CachingDiagnostics.NoCachingFounded(key, executingContext, Context);

            executedContext = await next();
            dumpStream.Position = 0;
            await dumpStream.CopyToAsync(originalBody);

            cacheEntry = new ResponseCacheEntry(response.ContentType ?? string.Empty, dumpStream.ToArray().AsMemory(), Context.Duration);
        }
        finally
        {
            response.Body = originalBody;
        }

        return await CheckAndStoreCacheAsync(executingContext, executedContext, key, cacheEntry);
    }

    /// <summary>
    /// 执行请求
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    protected virtual Task ExecutingRequestAsync(ResourceExecutingContext context, ResourceExecutionDelegate next, string key)
    {
        return DumpAndCacheResponseAsync(context, next, key);
    }

    #endregion Protected 方法
}
