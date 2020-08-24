using System;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.Internal;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Lockers
{
#pragma warning disable CA1001 // 具有可释放字段的类型应该是可释放的

    /// <summary>
    /// 默认基于Action的http请求执行锁定器 - ResourceFilter
    /// <para/>
    /// ！！实现 <see cref="IDisposable"/> 接口将导致Transient和Scoped实例在离开作用域时被释放
    /// </summary>
    public sealed class DefaultActionSingleResourceExecutingLocker : IActionSingleResourceExecutingLocker, IResourceExecutingLocker
#pragma warning restore CA1001 // 具有可释放字段的类型应该是可释放的
    {
        private readonly LocalCacheableLockPool<string, ResponseCacheEntry> _localCacheableLockPool = new LocalCacheableLockPool<string, ResponseCacheEntry>(() => new LocalCacheableLock<ResponseCacheEntry>(ResponseCachingConstants.MinCacheAvailableMilliseconds));

        /// <inheritdoc/>
        public void Dispose() => _localCacheableLockPool.Dispose();

        /// <inheritdoc/>
        public Task ProcessCacheWithLockAsync(string cacheKey, ResourceExecutingContext executingContext, Func<ResponseCacheEntry, Task> cacheAvailableFunc, Func<Task<ResponseCacheEntry>> cacheUnAvailableFunc)
        {
            return _localCacheableLockPool.GetLock(executingContext.ActionDescriptor.Id).LockRunAsync(cacheAvailableFunc, cacheUnAvailableFunc, executingContext.HttpContext.RequestAborted);
        }
    }
}