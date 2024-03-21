using Cuture.AspNetCore.ResponseCaching.Diagnostics;
using Cuture.AspNetCore.ResponseCaching.Internal;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Cuture.AspNetCore.ResponseCaching.Filters;

/// <summary>
/// 默认的基于ActionFilter的缓存过滤Filter
/// </summary>
public class DefaultActionCacheFilter : CacheFilterBase<ActionExecutingContext>, IAsyncActionFilter, IAsyncResourceFilter
{
    #region Protected 字段

    /// <summary>
    /// 空的缓存项信息
    /// </summary>
    protected static readonly (string? key, ResponseCacheEntry? cacheEntry) s_emptyCacheEntryInfo = new();

    #endregion Protected 字段

    #region Public 构造函数

    /// <summary>
    /// 默认的基于ActionFilter的缓存过滤Filter
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cachingDiagnosticsAccessor"></param>
    public DefaultActionCacheFilter(ResponseCachingContext context,
                                    CachingDiagnosticsAccessor cachingDiagnosticsAccessor)
        : base(context, cachingDiagnosticsAccessor)
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

        await ExecutingRequestInActionFilterAsync(executingContext, next, key);
    }

    /// <inheritdoc/>
    public async Task OnResourceExecutionAsync(ResourceExecutingContext executingContext, ResourceExecutionDelegate next)
    {
        CachingDiagnostics.StartProcessingCache(executingContext, Context);
        try
        {
            var executedContext = await next();

            await AfterActionExecutedInResourceFilterAsync(executingContext, executedContext);
        }
        finally
        {
            CachingDiagnostics.EndProcessingCache(executingContext, Context);
        }
    }

    #endregion Public 方法

    #region Protected 方法

    /// <summary>
    /// 在当前 <see cref="DefaultActionCacheFilter"/> 中的 <see cref="IAsyncResourceFilter.OnResourceExecutionAsync(ResourceExecutingContext, ResourceExecutionDelegate)"/> 方法执行后，执行此方法
    /// </summary>
    /// <param name="executingContext"></param>
    /// <param name="executedContext"></param>
    /// <returns></returns>
    protected virtual Task AfterActionExecutedInResourceFilterAsync(ResourceExecutingContext executingContext, ResourceExecutedContext executedContext)
    {
        return RestoreResponseStreamAndDumpContentAsync(executingContext);
    }

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
        var dumpStream = executingContext.HttpContext.RequestServices.GetRequiredService<IResponseDumpStreamFactory>().Create(Context.DumpStreamCapacity);

        try
        {
            response.Body = dumpStream;

            CachingDiagnostics.NoCachingFounded(key, executingContext, Context);

            var executedContext = await next();

            if (Context.CacheDeterminer.CanCaching(executedContext))
            {
                executingContext.HttpContext.Items.Add(ResponseCachingConstants.ResponseCachingResponseDumpContextKey, new ResponseDumpContext(key, dumpStream, originalBody));

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
    /// 在<see cref="IAsyncActionFilter"/>中执行请求
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    protected virtual Task ExecutingRequestInActionFilterAsync(ActionExecutingContext context, ActionExecutionDelegate next, string key)
    {
        return ExecutingAndReplaceResponseStreamAsync(context, next, key);
    }

    /// <summary>
    /// 还原响应流，并Dump出响应内容
    /// </summary>
    /// <param name="executingContext"></param>
    /// <returns></returns>
    protected async Task<(string? key, ResponseCacheEntry? cacheEntry)> RestoreResponseStreamAndDumpContentAsync(ResourceExecutingContext executingContext)
    {
        if (executingContext.HttpContext.Items.TryGetValue(ResponseCachingConstants.ResponseCachingResponseDumpContextKey, out var dumpContextObject)
            && dumpContextObject is ResponseDumpContext dumpContext)
        {
            var dumpStream = dumpContext.DumpStream;
            var key = dumpContext.Key;
            var originalBody = dumpContext.OriginalStream;
            try
            {
                dumpStream.Position = 0;
                await dumpStream.CopyToAsync(originalBody);

                var responseBody = dumpStream.ToArray().AsMemory();

                if (responseBody.Length <= Context.MaxCacheableResponseLength)
                {
                    var cacheEntry = new ResponseCacheEntry(executingContext.HttpContext.Response.ContentType ?? string.Empty, responseBody, Context.Duration);

                    cacheEntry = await StoreCacheAsync(executingContext, key, cacheEntry);
                    return (key, cacheEntry);
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

        return s_emptyCacheEntryInfo;
    }

    #endregion Protected 方法
}
