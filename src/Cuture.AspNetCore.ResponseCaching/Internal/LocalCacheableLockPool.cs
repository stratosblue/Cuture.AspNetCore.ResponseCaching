using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cuture.AspNetCore.ResponseCaching.Internal
{
    /// <summary>
    /// 可本地缓存锁池
    /// </summary>
    /// <typeparam name="TKey">key类型</typeparam>
    /// <typeparam name="TPayload">缓存内容类型</typeparam>
    internal sealed class LocalCacheableLockPool<TKey, TPayload> : IDisposable where TPayload : class where TKey : notnull
    {
        #region Private 字段

        private readonly Func<LocalCacheableLock<TPayload>> _lockFactory;
        private Dictionary<TKey, LocalCacheableLock<TPayload>> _allLocks = new();
        private bool _isDisposed = false;

        #endregion Private 字段

        #region Public 构造函数

        /// <summary>
        /// 可本地缓存锁池
        /// </summary>
        /// <param name="lockFactory"></param>
        public LocalCacheableLockPool(Func<LocalCacheableLock<TPayload>> lockFactory)
        {
            _lockFactory = lockFactory ?? throw new ArgumentNullException(nameof(lockFactory));
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;

            var all = _allLocks;
            _allLocks = null!;
            var tmpLockStates = all.ToImmutableArray();
            foreach (var item in tmpLockStates)
            {
                item.Value.Dispose();
            }
        }

        /// <summary>
        /// 获取锁
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LocalCacheableLock<TPayload> GetLock(TKey key)
        {
            CheckDisposed();

            LocalCacheableLock<TPayload> @lock;
            lock (_allLocks)
            {
                if (!_allLocks.TryGetValue(key, out @lock!))
                {
                    @lock = _lockFactory();
                    _allLocks.Add(key, @lock);
                }
                Interlocked.Add(ref @lock.ReferenceCount, 1);
            }

            return @lock;
        }

        public void Return(TKey key, LocalCacheableLock<TPayload> item)
        {
            if (Interlocked.Add(ref item.ReferenceCount, -1) == 0)
            {
                lock (_allLocks)
                {
                    if (_allLocks.TryGetValue(key, out item!))
                    {
                        if (item is not null
                           && item.ReferenceCount == 0)
                        {
                            _allLocks.Remove(key);
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                item?.Dispose();
            }
        }

        #endregion Public 方法

        #region Private 方法

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(LocalCacheableLockPool<TKey, TPayload>));
            }
        }

        #endregion Private 方法
    }
}