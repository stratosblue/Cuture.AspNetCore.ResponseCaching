using Cuture.AspNetCore.ResponseCaching.Diagnostics;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Filters;

/// <summary>
/// 默认的基于ActionFilter的缓存过滤Filter
/// </summary>
public class DefaultLockedActionCacheFilter : DefaultActionCacheFilter
{
    #region Private 字段

    private readonly IExecutingLockPool<ResponseCacheEntry> _executingLockPool;

    #endregion Private 字段

    #region Public 构造函数

    /// <summary>
    /// 默认的基于ActionFilter的缓存过滤Filter
    /// </summary>
    /// <param name="context"></param>
    /// <param name="executingLockPool"></param>
    /// <param name="cachingDiagnosticsAccessor"></param>
    public DefaultLockedActionCacheFilter(ResponseCachingContext context,
                                          IExecutingLockPool<ResponseCacheEntry> executingLockPool,
                                          CachingDiagnosticsAccessor cachingDiagnosticsAccessor)
        : base(context, cachingDiagnosticsAccessor)
    {
        _executingLockPool = executingLockPool ?? throw new ArgumentNullException(nameof(executingLockPool));
    }

    #endregion Public 构造函数

    #region Protected 方法

    /// <inheritdoc/>
    protected override async Task AfterActionExecutedInResourceFilterAsync(ResourceExecutingContext executingContext, ResourceExecutedContext executedContext)
    {
        if (executingContext.HttpContext.Response.HasStarted)
        {
            return;
        }
        if (!executingContext.HttpContext.Items.TryGetValue(ResponseCachingConstants.ResponseCachingExecutingLockKey, out var lockObject))
        {
            return;
        }
        var @lock = (IExecutingLock<ResponseCacheEntry>?)lockObject ?? throw new ResponseCachingException($"Error can not get {ResponseCachingConstants.ResponseCachingExecutingLockKey} in HttpContext.Items");

        try
        {
            var (key, cacheEntry) = await RestoreResponseStreamAndDumpContentAsync(executingContext);
            if (key is not null
                && cacheEntry is not null)
            {
                await CheckAndStoreCacheAsync(executingContext, executedContext, key, cacheEntry);
                @lock.SetLocalCache(key, cacheEntry, DateTimeOffset.Now.ToUnixTimeMilliseconds() + Context.DurationMilliseconds);
            }
        }
        finally
        {
            @lock.Release();
            _executingLockPool.Return(@lock);
        }
    }

    /// <inheritdoc/>
    protected override async Task ExecutingRequestInActionFilterAsync(ActionExecutingContext context, ActionExecutionDelegate next, string key)
    {
        var @lock = _executingLockPool.GetLock(key);
        if (@lock is null)
        {
            CachingDiagnostics.CannotExecutionThroughLock(key, context, Context);
            await Context.OnCannotExecutionThroughLock(key, context, () => next());
            return;
        }

        bool gotLock = false;

        try
        {
            var waitTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            gotLock = await @lock.WaitAsync(Context.LockMillisecondsTimeout, context.HttpContext.RequestAborted);

            if (!gotLock)   //没有获取到锁
            {
                _executingLockPool.Return(@lock);
                await Context.OnExecutionLockTimeout(key, context, () => next());
                return;
            }

            if (@lock.TryGetLocalCache(key, waitTime, out var responseCacheEntry))
            {
                _ = WriteCacheToResponseWithInterceptorAsync(context, responseCacheEntry);
                @lock.Release();
                _executingLockPool.Return(@lock);
                return;
            }
            else
            {
                context.HttpContext.Items.Add(ResponseCachingConstants.ResponseCachingExecutingLockKey, @lock);
            }
        }
        catch
        {
            if (gotLock)
            {
                @lock.Release();
            }
            _executingLockPool.Return(@lock);
            throw;
        }

        //执行请求的后续处理逻辑
        await ExecutingAndReplaceResponseStreamAsync(context, next, key);
    }

    #endregion Protected 方法
}
