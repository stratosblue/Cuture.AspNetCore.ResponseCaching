using Microsoft.Extensions.ObjectPool;

namespace Cuture.AspNetCore.ResponseCaching.ExecutingLock;

internal abstract class ExecutingLockLifecycleExecutorBase<TCachePayload, TExecutingLock>
    : IObjectLifecycleExecutor<TExecutingLock>
    where TExecutingLock : IExecutingLock<TCachePayload>
    where TCachePayload : class
{
    #region Private 字段

    private readonly INakedBoundedObjectPool<SemaphoreSlim> _semaphorePool;

    #endregion Private 字段

    #region Public 构造函数

    public ExecutingLockLifecycleExecutorBase(INakedBoundedObjectPool<SemaphoreSlim> semaphorePool)
    {
        _semaphorePool = semaphorePool ?? throw new ArgumentNullException(nameof(semaphorePool));
    }

    #endregion Public 构造函数

    #region Public 方法

    public TExecutingLock? Create()
    {
        var semaphore = _semaphorePool.Rent();
        if (semaphore is not null)
        {
            return Create(semaphore);
        }
        return default;
    }

    public void Destroy(TExecutingLock item)
    {
        if (item is ExecutingLockBase<TCachePayload> executingLockBase
            && executingLockBase.Semaphore is not null)
        {
            _semaphorePool.Return(executingLockBase.Semaphore);
            executingLockBase.Semaphore = null!;
        }
    }

    public bool Reset(TExecutingLock item)
    {
        if (item is ExecutingLockBase<TCachePayload> executingLockBase)
        {
            if (executingLockBase.ReferenceCount > 0)
            {
                throw new InvalidOperationException($"{nameof(ExecutingLockBase<TCachePayload>)} in use.");
            }
            while (executingLockBase.Semaphore.CurrentCount < 1)
            {
                executingLockBase.Semaphore.Release();
            }
            Interlocked.Exchange(ref executingLockBase.ReferenceCount, 0);
            executingLockBase.SetLocalCache(string.Empty, null, 0L);
            return true;
        }
        return false;
    }

    #endregion Public 方法

    #region Protected 方法

    protected abstract TExecutingLock Create(SemaphoreSlim semaphore);

    #endregion Protected 方法
}
