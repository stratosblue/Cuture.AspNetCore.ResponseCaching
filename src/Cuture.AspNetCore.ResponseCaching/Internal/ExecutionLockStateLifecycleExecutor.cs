using System;
using System.Threading;

using Microsoft.Extensions.ObjectPool;

namespace Cuture.AspNetCore.ResponseCaching.Internal
{
    /// <summary>
    /// 执行锁状态池
    /// </summary>
    internal sealed class ExecutionLockStateLifecycleExecutor<TStatePayload> : IObjectLifecycleExecutor<ExecutionLockState<TStatePayload>> where TStatePayload : class
    {
        #region Private 字段

        private readonly INakedBoundedObjectPool<SemaphoreSlim> _semaphorePool;

        #endregion Private 字段

        #region Public 构造函数

        public ExecutionLockStateLifecycleExecutor(INakedBoundedObjectPool<SemaphoreSlim> semaphorePool)
        {
            _semaphorePool = semaphorePool ?? throw new ArgumentNullException(nameof(semaphorePool));
        }

        #endregion Public 构造函数

        #region Public 方法

        public ExecutionLockState<TStatePayload>? Create()
        {
            var semaphore = _semaphorePool.Rent();
            if (semaphore is not null)
            {
                return new ExecutionLockState<TStatePayload>(semaphore);
            }
            return null;
        }

        public void Destroy(ExecutionLockState<TStatePayload> item)
        {
            if (item.Lock is not null)
            {
                _semaphorePool.Return(item.Lock);
                item.Lock = null!;
            }
        }

        public bool Reset(ExecutionLockState<TStatePayload> item)
        {
            if (item.ReferenceCount > 0)
            {
                throw new InvalidOperationException($"{nameof(ExecutionLockState<TStatePayload>)} in use.");
            }
            while (item.Lock.CurrentCount < 1)
            {
                item.Lock.Release();
            }
            item.ReferenceCount = 0;
            item.LocalCachedPayload = null;
            return true;
        }

        #endregion Public 方法
    }
}