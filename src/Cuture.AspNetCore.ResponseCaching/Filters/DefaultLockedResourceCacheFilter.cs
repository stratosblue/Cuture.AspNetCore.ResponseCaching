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
    public class DefaultLockedResourceCacheFilter : DefaultResourceCacheFilter
    {
        #region Private 字段

        private readonly IExecutingLockPool<ResponseCacheEntry> _executingLockPool;

        #endregion Private 字段

        #region Public 构造函数

        /// <summary>
        /// 默认的基于ResourceFilter的缓存过滤Filter
        /// </summary>
        /// <param name="context"></param>
        /// <param name="executingLockPool"></param>
        /// <param name="cachingDiagnosticsAccessor"></param>
        public DefaultLockedResourceCacheFilter(ResponseCachingContext context,
                                                IExecutingLockPool<ResponseCacheEntry> executingLockPool,
                                                CachingDiagnosticsAccessor cachingDiagnosticsAccessor)
            : base(context, cachingDiagnosticsAccessor)
        {
            _executingLockPool = executingLockPool ?? throw new ArgumentNullException(nameof(executingLockPool));
        }

        #endregion Public 构造函数

        #region Protected 方法

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override async Task ExecutingRequestAsync(ResourceExecutingContext context, ResourceExecutionDelegate next, string key)
        {
            var @lock = _executingLockPool.GetLock(key);
            if (@lock is null)
            {
                CachingDiagnostics.CannotExecutionThroughLock(key, context, Context);
                await Context.OnCannotExecutionThroughLock(key, context);
                return;
            }

            //TODO 等待超时时间 - 测试
            bool gotLock = false;

            try
            {
                var waitTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                gotLock = await @lock.WaitAsync(Context.LockMillisecondsTimeout, context.HttpContext.RequestAborted);

                if (!gotLock)   //没有获取到锁
                {
                    await Context.OnExecutionLockTimeout(key, context, () => next());
                    return;
                }

                if (@lock.TryGetLocalCache(key, waitTime, out var responseCacheEntry))
                {
                    _ = WriteCacheToResponseWithInterceptorAsync(context, responseCacheEntry);
                }
                else
                {
                    responseCacheEntry = await DumpAndCacheResponseAsync(context, next, key);
                    if (responseCacheEntry is not null)
                    {
                        @lock.SetLocalCache(key, responseCacheEntry, DateTimeOffset.Now.ToUnixTimeMilliseconds() + Context.DurationMilliseconds);
                    }
                }
            }
            finally
            {
                if (gotLock)
                {
                    @lock.Release();
                }
                _executingLockPool.Return(@lock);
            }
        }

        #endregion Protected 方法
    }
}