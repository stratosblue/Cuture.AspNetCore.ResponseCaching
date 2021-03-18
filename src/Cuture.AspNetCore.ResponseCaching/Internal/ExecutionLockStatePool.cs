using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;

using Microsoft.Extensions.ObjectPool;

namespace Cuture.AspNetCore.ResponseCaching.Internal
{
    /// <summary>
    /// 执行锁状态
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    internal class ExecutionLockState<TPayload> where TPayload : class
    {
        #region Public 字段

        /// <summary>
        /// 引用计数
        /// </summary>
        public int ReferenceCount = 0;

        #endregion Public 字段

        #region Public 属性

        /// <summary>
        /// 本地缓存数据
        /// </summary>
        public volatile TPayload? LocalCachedPayload;

        /// <summary>
        /// 本地缓存数据过期时间
        /// </summary>
        public long LocalCachedPayloadExpireTime;

        public SemaphoreSlim Lock { get; set; }

        #endregion Public 属性

        #region Public 构造函数

        public ExecutionLockState(SemaphoreSlim semaphore)
        {
            Lock = semaphore;
        }

        #endregion Public 构造函数
    }

    /// <summary>
    /// 执行锁状态池
    /// </summary>
    internal sealed class ExecutionLockStatePool<TStatePayload> : IDisposable where TStatePayload : class
    {
        #region Private 字段

        private readonly IDirectBoundedObjectPool<SemaphoreSlim> _semaphorePool;
        private readonly IRecyclePool<SemaphoreSlim> _semaphoreRecyclePool;
        private Dictionary<string, ExecutionLockState<TStatePayload>> _allLockState = new();
        private bool _disposedValue;

        #endregion Private 字段

        #region Public 构造函数

        public ExecutionLockStatePool(IDirectBoundedObjectPool<SemaphoreSlim> semaphorePool,
                                      IRecyclePool<SemaphoreSlim> semaphoreRecyclePool)
        {
            _semaphorePool = semaphorePool;
            _semaphoreRecyclePool = semaphoreRecyclePool;
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <summary>
        /// 获取锁
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ExecutionLockState<TStatePayload>? GetLock(string key)
        {
            CheckDisposed();

            ExecutionLockState<TStatePayload> state;
            lock (_allLockState)
            {
                if (!_allLockState.TryGetValue(key, out state!))
                {
                    var semaphore = _semaphorePool.Rent();
                    if (semaphore is null)
                    {
                        return null;
                    }
                    state = new ExecutionLockState<TStatePayload>(semaphore);
                    _allLockState.Add(key, state);
                }
                Interlocked.Add(ref state.ReferenceCount, 1);
            }

            return state;
        }

        public void Return(string key, ExecutionLockState<TStatePayload> item)
        {
            if (Interlocked.Add(ref item.ReferenceCount, -1) == 0)
            {
                lock (_allLockState)
                {
                    if (_allLockState.TryGetValue(key, out item!))
                    {
                        if (item is not null
                           && item.ReferenceCount == 0)
                        {
                            _allLockState.Remove(key);
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                _semaphoreRecyclePool.Return(item.Lock);
            }
        }

        #endregion Public 方法

        #region Private 方法

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckDisposed()
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(nameof(ExecutionLockStatePool<TStatePayload>));
            }
        }

        #region Dispose

        ~ExecutionLockStatePool()
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
                var all = _allLockState;
                _allLockState = null!;
                var tmpLockStates = all.ToImmutableArray();
                foreach (var item in tmpLockStates)
                {
                    var semaphore = item.Value.Lock;
                    item.Value.Lock = null!;
                    _semaphoreRecyclePool.Return(semaphore);
                }
                _disposedValue = true;
            }
        }

        #endregion Dispose

        #endregion Private 方法
    }
}