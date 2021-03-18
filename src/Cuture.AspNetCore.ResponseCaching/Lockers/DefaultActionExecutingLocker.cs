using System;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.Internal;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseCaching.Lockers
{
    /// <summary>
    /// 默认基于缓存键的http请求执行锁定器 - ActionFilter
    /// <para/>
    /// </summary>
    internal sealed class DefaultActionExecutingLocker : ExclusiveExecutingLockerBase<IActionResult>, ICacheKeySingleActionExecutingLocker, IDisposable
    {
        #region Private 字段

        private readonly ExecutionLockStatePool<IActionResult> _executionLockStatePool;

        #endregion Private 字段

        #region Public 属性

        /// <inheritdoc/>
        public bool IsShared { get; private set; } = true;

        #endregion Public 属性

        #region Public 构造函数

        public DefaultActionExecutingLocker(IOptions<ResponseCachingOptions> options, ExecutionLockStatePool<IActionResult> executionLockStatePool) : base(options)
        {
            _executionLockStatePool = executionLockStatePool;
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <summary>
        /// 释放相关资源
        /// </summary>
        public void Dispose() => _executionLockStatePool.Dispose();

        /// <inheritdoc/>
        public async Task<bool> ProcessCacheWithLockAsync(string cacheKey, ActionExecutingContext executingContext, Func<IActionResult, Task> cacheAvailableFunc, Func<Task<IActionResult?>> cacheUnAvailableFunc)
        {
            var lockState = _executionLockStatePool.GetLock(cacheKey);
            if (lockState is not null)
            {
                try
                {
                    await LockRunAsync(lockState, cacheAvailableFunc, cacheUnAvailableFunc, executingContext.HttpContext.RequestAborted);
                }
                finally
                {
                    _executionLockStatePool.Return(cacheKey, lockState);
                }
                return true;
            }
            return false;
        }

        #endregion Public 方法
    }
}