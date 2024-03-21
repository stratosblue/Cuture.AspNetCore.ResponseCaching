using Microsoft.Extensions.ObjectPool;

namespace Cuture.AspNetCore.ResponseCaching.ExecutingLock;

internal sealed class ExclusiveExecutingLockLifecycleExecutor<TCachePayload>
    : ExecutingLockLifecycleExecutorBase<TCachePayload, ExclusiveExecutingLock<TCachePayload>>
    where TCachePayload : class
{
    #region Public 构造函数

    public ExclusiveExecutingLockLifecycleExecutor(INakedBoundedObjectPool<SemaphoreSlim> semaphorePool) : base(semaphorePool)
    {
    }

    #endregion Public 构造函数

    #region Public 方法

    protected override ExclusiveExecutingLock<TCachePayload> Create(SemaphoreSlim semaphore)
        => new(semaphore);

    #endregion Public 方法
}
