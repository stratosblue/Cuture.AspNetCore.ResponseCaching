using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;

using Microsoft.Extensions.ObjectPool;

namespace Cuture.AspNetCore.ResponseCaching
{
    internal class ExecutingLockPool<TCachePayload, TExecutingLock>
        : IExecutingLockPool<TCachePayload>
        where TExecutingLock : ExecutingLockBase<TCachePayload>
        where TCachePayload : class
    {
        #region Private 字段

        private readonly Dictionary<string, TExecutingLock> _allLocks = new();
        private readonly INakedBoundedObjectPool<TExecutingLock> _lockPool;
        private bool _disposedValue;

        #endregion Private 字段

        #region Public 构造函数

        public ExecutingLockPool(INakedBoundedObjectPool<TExecutingLock> lockPool)
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

            TExecutingLock? @lock;
            lock (_allLocks)
            {
                if (!_allLocks.TryGetValue(lockKey, out @lock))
                {
                    @lock = _lockPool.Rent();
                    if (@lock is null)
                    {
                        return null;
                    }
                    @lock.LockKey = lockKey;
                    _allLocks.Add(lockKey, @lock);
                }
                Interlocked.Add(ref @lock.ReferenceCount, 1);
            }

            return @lock;
        }

        /// <inheritdoc/>
        public void Return(IExecutingLock<TCachePayload> item)
        {
            if (item is TExecutingLock executingLock
                && Interlocked.Add(ref executingLock.ReferenceCount, -1) == 0)
            {
                //HACK 确认是否有这种情况
                var key = executingLock.LockKey ?? throw new ResponseCachingException($"Error state of {typeof(IExecutingLock<TCachePayload>)} . No CacheKey.");

                lock (_allLocks)
                {
                    if (_allLocks.TryGetValue(key, out executingLock!))
                    {
                        if (executingLock.ReferenceCount == 0)
                        {
                            _allLocks.Remove(key);
                        }
                        else
                        {
                            return;
                        }
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

        #endregion Private 方法

        #region Dispose

        ~ExecutingLockPool()
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
                var all = _allLocks;
                var tmpLockStates = all.ToImmutableArray();
                foreach (var item in tmpLockStates)
                {
                    _lockPool.Return(item.Value);
                }
                _disposedValue = true;
            }
        }

        #endregion Dispose
    }
}