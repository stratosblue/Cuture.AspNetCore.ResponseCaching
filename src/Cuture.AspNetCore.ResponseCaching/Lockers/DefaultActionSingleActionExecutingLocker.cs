using System;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.Internal;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Lockers
{
#pragma warning disable CA1001 // 具有可释放字段的类型应该是可释放的

    /// <summary>
    /// 默认基于Action的http请求执行锁定器 - ActionFilter
    /// <para/>
    /// ！！实现 <see cref="IDisposable"/> 接口将导致Transient和Scoped实例在离开作用域时被释放
    /// </summary>
    public sealed class DefaultActionSingleActionExecutingLocker : IActionSingleActionExecutingLocker, IActionExecutingLocker
#pragma warning restore CA1001 // 具有可释放字段的类型应该是可释放的
    {
        private readonly LocalCacheableLockPool<string, IActionResult> _localCacheableLockPool = new LocalCacheableLockPool<string, IActionResult>(() => new LocalCacheableLock<IActionResult>(ResponseCachingConstants.MinCacheAvailableMilliseconds));

        /// <inheritdoc/>
        public void Dispose() => _localCacheableLockPool.Dispose();

        /// <inheritdoc/>
        public Task ProcessCacheWithLockAsync(string cacheKey, ActionExecutingContext executingContext, Func<IActionResult, Task> cacheAvailableFunc, Func<Task<IActionResult>> cacheUnAvailableFunc)
        {
            return _localCacheableLockPool.GetLock(executingContext.ActionDescriptor.Id).LockRunAsync(cacheAvailableFunc, cacheUnAvailableFunc, executingContext.HttpContext.RequestAborted);
        }
    }
}