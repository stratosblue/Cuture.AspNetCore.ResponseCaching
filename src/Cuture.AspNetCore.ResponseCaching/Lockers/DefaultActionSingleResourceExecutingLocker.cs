using System;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.Internal;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseCaching.Lockers
{
    /// <summary>
    /// 默认基于Action的http请求执行锁定器 - ResourceFilter
    /// </summary>
    internal sealed class DefaultActionSingleResourceExecutingLocker : SharedExecutingLockerBase<ResponseCacheEntry>, IActionSingleResourceExecutingLocker, IDisposable
    {
        #region Private 字段

        private readonly ExecutionLockStatePool<ResponseCacheEntry> _executionLockStatePool;

        #endregion Private 字段

        #region Public 属性

        /// <inheritdoc/>
        public bool IsShared { get; private set; } = true;

        #endregion Public 属性

        #region Public 构造函数

        public DefaultActionSingleResourceExecutingLocker(IOptions<ResponseCachingOptions> options, ExecutionLockStatePool<ResponseCacheEntry> executionLockStatePool) : base(options)
        {
            _executionLockStatePool = executionLockStatePool;
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public void Dispose() => _executionLockStatePool.Dispose();

        /// <inheritdoc/>
        public async Task<bool> ProcessCacheWithLockAsync(string cacheKey, ResourceExecutingContext executingContext, Func<ResponseCacheEntry, Task> cacheAvailableFunc, Func<Task<ResponseCacheEntry?>> cacheUnAvailableFunc)
        {
            var lockState = _executionLockStatePool.GetLock(executingContext.ActionDescriptor.Id);
            if (lockState is not null)
            {
                try
                {
                    await LockRunAsync(cacheKey, lockState, cacheAvailableFunc, cacheUnAvailableFunc, executingContext.HttpContext.RequestAborted);
                }
                finally
                {
                    _executionLockStatePool.Return(executingContext.ActionDescriptor.Id, lockState);
                }
                return true;
            }
            return false;
        }

        #endregion Public 方法
    }
}