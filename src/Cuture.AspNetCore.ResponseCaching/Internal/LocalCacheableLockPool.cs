using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading;

namespace Cuture.AspNetCore.ResponseCaching.Internal
{
    /// <summary>
    /// 可本地缓存锁池
    /// </summary>
    /// <typeparam name="TKey">key类型</typeparam>
    /// <typeparam name="TPayload">缓存内容类型</typeparam>
    public sealed class LocalCacheableLockPool<TKey, TPayload> : IDisposable where TPayload : class
    {
        #region Private 字段

        private readonly Func<LocalCacheableLock<TPayload>> _lockFactory;
        private ConcurrentDictionary<TKey, Lazy<LockState<TPayload>>> _allLockStates = new ConcurrentDictionary<TKey, Lazy<LockState<TPayload>>>();

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
        /// 清理锁字典内的空项
        /// </summary>
        public void Clean()
        {
            lock (_allLockStates)
            {
                var tmpLockStates = _allLockStates.ToImmutableArray();
                foreach (var item in tmpLockStates)
                {
                    if (!item.Value.IsValueCreated)
                    {
                        continue;
                    }
                    var lockState = item.Value.Value;
                    bool lockTaken = false;
                    try
                    {
                        lockState.UseLock.Enter(ref lockTaken);

                        var lockWeakReference = lockState.WeakReference;
                        if (!lockWeakReference.TryGetTarget(out var _))
                        {
                            lockState.InvalidMarked = true;
                            _allLockStates.TryRemove(item.Key, out var _);
                        }
                    }
                    finally
                    {
                        if (lockTaken)
                        {
                            lockState.UseLock.Exit(false);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            var all = _allLockStates;
            _allLockStates = null;
            var tmpLockStates = all.ToImmutableArray();
            foreach (var item in tmpLockStates)
            {
                if (!item.Value.IsValueCreated)
                {
                    continue;
                }

                if (item.Value.Value.WeakReference.TryGetTarget(out var @lock))
                {
                    @lock.Dispose();
                }
            }
        }

        /// <summary>
        /// 获取锁
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LocalCacheableLock<TPayload> GetLock(TKey key)
        {
            var lazyLockState = _allLockStates.GetOrAdd(key, (innerKey) =>
            {
                return new Lazy<LockState<TPayload>>(() => new LockState<TPayload>(new WeakReference<LocalCacheableLock<TPayload>>(_lockFactory())));
            });

            var lockState = lazyLockState.Value;

            bool lockTaken = false;
            try
            {
                lockState.UseLock.Enter(ref lockTaken);

                if (lockState.InvalidMarked)
                {
                    if (lockTaken)
                    {
                        lockState.UseLock.Exit(false);
                    }
                    lockTaken = false;
                    return GetLock(key);
                }

                var lockWeakReference = lockState.WeakReference;
                if (!lockWeakReference.TryGetTarget(out var @lock))
                {
                    @lock = _lockFactory();
                    lockWeakReference.SetTarget(@lock);
                }
                return @lock;
            }
            finally
            {
                if (lockTaken)
                {
                    lockState.UseLock.Exit(false);
                }
            }
        }

        #endregion Public 方法
    }

    /// <summary>
    /// 锁状态
    /// </summary>
    internal class LockState<TPayload> where TPayload : class
    {
        #region Public 字段

        /// <summary>
        /// 无效标记，不应该继续使用该对象
        /// </summary>
        public volatile bool InvalidMarked;

        /// <summary>
        /// 使用锁
        /// </summary>
        public SpinLock UseLock = new SpinLock(false);

        #endregion Public 字段

        #region Public 属性

        /// <summary>
        /// 弱引用
        /// </summary>
        public WeakReference<LocalCacheableLock<TPayload>> WeakReference { get; set; }

        #endregion Public 属性

        #region Public 构造函数

        /// <summary>
        /// 锁状态
        /// </summary>
        /// <param name="weakReference"></param>
        public LockState(WeakReference<LocalCacheableLock<TPayload>> weakReference)
        {
            WeakReference = weakReference;
        }

        #endregion Public 构造函数
    }
}