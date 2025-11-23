using Microsoft.Extensions.ObjectPool;

namespace Cuture.AspNetCore.ResponseCaching.ExecutingLock;

internal sealed class ExclusiveExecutingLockLifecycleExecutor<TCachePayload>(INakedBoundedObjectPool<SemaphoreSlim> semaphorePool)
    : ExecutingLockLifecycleExecutorBase<TCachePayload, ExclusiveExecutingLock<TCachePayload>>(semaphorePool)
    where TCachePayload : class
{
    #region Public 方法

    protected override ExclusiveExecutingLock<TCachePayload> Create(SemaphoreSlim semaphore)
        => new(semaphore);

    #endregion Public 方法
}
