using System;
using System.Runtime.CompilerServices;
using System.Threading;

using Microsoft.Extensions.ObjectPool;

namespace Cuture.AspNetCore.ResponseCaching;

internal class SingleLockExecutingLockPool<TCachePayload, TExecutingLock>
    : IExecutingLockPool<TCachePayload>
    where TExecutingLock : ExecutingLockBase<TCachePayload>
    where TCachePayload : class
{
    #region Private 类

    private class LockHolder
    {
        #region Public 属性

        public TExecutingLock? Lock { get; set; }

        #endregion Public 属性
    }

    #endregion Private 类

    #region Private 字段

    private readonly INakedBoundedObjectPool<TExecutingLock> _lockPool;
    private readonly LockHolder lockHolder = new();
    private bool _disposedValue;

    #endregion Private 字段

    #region Public 构造函数

    public SingleLockExecutingLockPool(INakedBoundedObjectPool<TExecutingLock> lockPool)
    {
        _lockPool = lockPool ?? throw new ArgumentNullException(nameof(lockPool));
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <summary>
    /// 获取锁
    /// </summary>
    /// <param name="lockKey"></param>
    /// <returns></returns>
    public IExecutingLock<TCachePayload>? GetLock(string lockKey)
    {
        CheckDisposed();

        lock (lockHolder)
        {
            if (lockHolder.Lock is not TExecutingLock @lock)
            {
                @lock = _lockPool.Rent()!;
                if (@lock is null)
                {
                    return null;
                }
                lockHolder.Lock = @lock;
            }
            Interlocked.Add(ref @lock.ReferenceCount, 1);
            return @lock;
        }
    }

    public void Return(IExecutingLock<TCachePayload> item)
    {
        if (item is TExecutingLock executingLock
            && ReferenceEquals(executingLock, lockHolder.Lock)
            && Interlocked.Add(ref executingLock.ReferenceCount, -1) == 0)
        {
            lock (lockHolder)
            {
                if (lockHolder.Lock != null
                    && executingLock.ReferenceCount == 0)
                {
                    lockHolder.Lock = null;
                }
                else
                {
                    return;
                }
            }
            executingLock.LockKey = null;
            _lockPool.Return(executingLock);
        }
    }

    #endregion Public 方法

    #region Private 方法

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckDisposed()
    {
        if (_disposedValue)
        {
            throw new ObjectDisposedException(nameof(ExecutingLockPool<TCachePayload, TExecutingLock>));
        }
    }

    #region Dispose

    ~SingleLockExecutingLockPool()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            var @lock = lockHolder.Lock;
            lockHolder.Lock = null;
            if (@lock != null)
            {
                _lockPool.Return(@lock);
            }
            _disposedValue = true;
        }
    }

    #endregion Dispose

    #endregion Private 方法
}