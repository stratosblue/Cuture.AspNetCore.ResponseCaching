using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseCaching.ExecutingLock;

internal sealed class SharedExecutingLockLifecycleExecutor<TCachePayload>
    : ExecutingLockLifecycleExecutorBase<TCachePayload, SharedExecutingLock<TCachePayload>>
    where TCachePayload : class
{
    #region Private 字段

    private readonly IMemoryCache _memoryCache;

    #endregion Private 字段

    #region Public 构造函数

    public SharedExecutingLockLifecycleExecutor(INakedBoundedObjectPool<SemaphoreSlim> semaphorePool, IOptions<ResponseCachingOptions> options) : base(semaphorePool)
    {
        _memoryCache = options.Value.LockedExecutionLocalResultCache;
    }

    #endregion Public 构造函数

    #region Public 方法

    protected override SharedExecutingLock<TCachePayload> Create(SemaphoreSlim semaphore)
        => new(semaphore, _memoryCache);

    #endregion Public 方法
}
