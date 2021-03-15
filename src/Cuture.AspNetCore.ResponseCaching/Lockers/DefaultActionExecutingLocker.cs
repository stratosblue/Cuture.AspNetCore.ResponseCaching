using System;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.Internal;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Lockers
{
    /// <summary>
    /// 默认基于缓存键的http请求执行锁定器 - ActionFilter
    /// <para/>
    /// </summary>
    public sealed class DefaultActionExecutingLocker : ICacheKeySingleActionExecutingLocker, IDisposable
    {
        #region Private 字段

        private readonly LocalCacheableLockPool<string, IActionResult> _localCacheableLockPool = new LocalCacheableLockPool<string, IActionResult>(() => new LocalCacheableLock<IActionResult>(ResponseCachingConstants.MinCacheAvailableMilliseconds));

        #endregion Private 字段

        #region Public 方法

        /// <inheritdoc/>
        public void Dispose() => _localCacheableLockPool.Dispose();

        /// <inheritdoc/>
        public async Task ProcessCacheWithLockAsync(string cacheKey, ActionExecutingContext executingContext, Func<IActionResult, Task> cacheAvailableFunc, Func<Task<IActionResult?>> cacheUnAvailableFunc)
        {
            var @lock = _localCacheableLockPool.GetLock(cacheKey);
            try
            {
                await @lock.LockRunAsync(cacheAvailableFunc, cacheUnAvailableFunc, executingContext.HttpContext.RequestAborted);
            }
            finally
            {
                _localCacheableLockPool.Return(cacheKey, @lock);
            }
        }

        #endregion Public 方法
    }
}