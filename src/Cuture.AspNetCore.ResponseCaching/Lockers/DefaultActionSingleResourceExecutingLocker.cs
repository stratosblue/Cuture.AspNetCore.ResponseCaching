using System;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.Internal;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Lockers
{
    /// <summary>
    /// 默认基于Action的http请求执行锁定器 - ResourceFilter
    /// </summary>
    public sealed class DefaultActionSingleResourceExecutingLocker : IActionSingleResourceExecutingLocker, IDisposable
    {
        #region Private 字段

        private readonly LocalCacheableLockPool<string, ResponseCacheEntry> _localCacheableLockPool = new LocalCacheableLockPool<string, ResponseCacheEntry>(() => new LocalCacheableLock<ResponseCacheEntry>(ResponseCachingConstants.MinCacheAvailableMilliseconds));

        #endregion Private 字段

        #region Public 方法

        /// <inheritdoc/>
        public void Dispose() => _localCacheableLockPool.Dispose();

        /// <inheritdoc/>
        public async Task ProcessCacheWithLockAsync(string cacheKey, ResourceExecutingContext executingContext, Func<ResponseCacheEntry, Task> cacheAvailableFunc, Func<Task<ResponseCacheEntry?>> cacheUnAvailableFunc)
        {
            var @lock = _localCacheableLockPool.GetLock(executingContext.ActionDescriptor.Id);
            try
            {
                await @lock.LockRunAsync(cacheAvailableFunc, cacheUnAvailableFunc, executingContext.HttpContext.RequestAborted);
            }
            finally
            {
                _localCacheableLockPool.Return(executingContext.ActionDescriptor.Id, @lock);
            }
        }

        #endregion Public 方法
    }
}