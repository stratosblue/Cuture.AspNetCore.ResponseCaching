using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.ObjectPool
{
    /// <summary>
    /// 有限大小的对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class DefaultBoundedObjectPool<T> : INakedBoundedObjectPool<T>, IBoundedObjectPool<T> where T : class
    {
        #region Private 字段

        private readonly IObjectLifecycleExecutor<T> _lifecycleExecutor;
        private readonly int _maximumPooled;
        private readonly int _minimumRetained;
        private readonly ConcurrentQueue<T> _objctQueue;
        private readonly IPoolReductionPolicy _poolReductionPolicy;
        private readonly TimeSpan _recycleInterval;
        private readonly object _syncRoot = new();
        private CancellationTokenSource _cleanTaskCancellationTokenSource;
        private bool _disposedValue;
        private long _lastCreateObjectTime = DateTimeOffset.UtcNow.Ticks;
        private volatile int _objectCount = 0;

        #endregion Private 字段

        #region Public 属性

        /// <inheritdoc/>
        public int AvailableCount => _objctQueue.Count;

        /// <inheritdoc/>
        public int PoolSize => _objectCount;

        #endregion Public 属性

        #region Public 构造函数

        /// <summary>
        /// <inheritdoc cref="DefaultBoundedObjectPool{T}"/>
        /// </summary>
        /// <param name="lifecycleExecutor">对象生命周期执行器</param>
        /// <param name="poolMaximum">最大对象数量</param>
        /// <param name="minimumRetained">最小保留对象数量</param>
        public DefaultBoundedObjectPool(IObjectLifecycleExecutor<T> lifecycleExecutor, BoundedObjectPoolOptions options)
        {
            _lifecycleExecutor = lifecycleExecutor ?? throw new ArgumentNullException(nameof(lifecycleExecutor));

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.MaximumPooled < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(options.MaximumPooled), $"{nameof(options.MaximumPooled)} can not be less than 1.");
            }
            if (options.MinimumRetained < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options.MinimumRetained), $"{nameof(options.MaximumPooled)} can not be less than 0.");
            }
            if (options.MaximumPooled < options.MinimumRetained)
            {
                throw new ArgumentException($"{nameof(options.MaximumPooled)} must be larger than {nameof(options.MinimumRetained)}.", nameof(options.MaximumPooled));
            }
            if (options.RecycleInterval < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(options.RecycleInterval), $"{nameof(options.RecycleInterval)} must be larger than 0.");
            }

            _maximumPooled = options.MaximumPooled;
            _minimumRetained = options.MinimumRetained;
            _recycleInterval = options.RecycleInterval;
            _poolReductionPolicy = options.PoolReductionPolicy ?? IPoolReductionPolicy.Default;

            _objctQueue = new ConcurrentQueue<T>();

            _cleanTaskCancellationTokenSource = new CancellationTokenSource();

            StartAutoReductionIdleObject();
        }

        #endregion Public 构造函数

        #region Private 析构函数

        ~DefaultBoundedObjectPool()
        {
            Dispose(disposing: false);
        }

        #endregion Private 析构函数

        #region Public 方法

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public IObjectOwner<T>? Rent()
        {
            CheckDisposed();
            var item = InternalRent();
            return item is null ? null : new ObjectOwner<T>(this, item);
        }

        /// <inheritdoc/>
        public void Return(T item)
        {
            if (item != null)
            {
                //HACK 当前Pool对象已经被释放了，执行释放逻辑 - 这样的行为会不会有什么问题，_lifecycleExecutor也不可用了？
                if (_disposedValue)
                {
                    _lifecycleExecutor.Destroy(item);
                    return;
                }

                if (_lifecycleExecutor.Reset(item))
                {
                    _objctQueue.Enqueue(item);
                }
                else
                {
                    _lifecycleExecutor.Destroy(item);
                    lock (_syncRoot)
                    {
                        _objectCount -= 1;
                    }
                }
            }
        }

        #endregion Public 方法

        #region Protected 方法

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                }

                _cleanTaskCancellationTokenSource.Cancel(true);
                _cleanTaskCancellationTokenSource.Dispose();
                _cleanTaskCancellationTokenSource = null!;

                _disposedValue = true;
            }
        }

        #endregion Protected 方法

        #region Private 方法

        /// <inheritdoc/>
        T? IDirectBoundedObjectPool<T>.Rent() => InternalRent();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckDisposed()
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(nameof(DefaultBoundedObjectPool<T>));
            }
        }

        private T? InternalRent()
        {
            if (!_objctQueue.TryDequeue(out var item))
            {
                Interlocked.Exchange(ref _lastCreateObjectTime, DateTimeOffset.UtcNow.Ticks);

                //进入锁之前进行一次判断，可以在满载情况下避免锁
                if (_objectCount >= _maximumPooled)
                {
                    return null;
                }

                lock (_syncRoot)
                {
                    if (_objectCount >= _maximumPooled)
                    {
                        return null;
                    }
                    _objectCount++;
                }
                item = _lifecycleExecutor.Create();
            }
            return item;
        }

        /// <summary>
        /// 开始自动缩减空闲对象
        /// </summary>
        private void StartAutoReductionIdleObject()
        {
            Task.Run(async () =>
            {
                var token = _cleanTaskCancellationTokenSource.Token;
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(_recycleInterval, token);

                    if (!_disposedValue
                        && _objectCount > _minimumRetained
                        && Interlocked.Read(ref _lastCreateObjectTime) < DateTimeOffset.UtcNow.Add(-_recycleInterval).Ticks)
                    {
                        var nextSize = _poolReductionPolicy.NextSize(_objectCount, _minimumRetained);
                        if (nextSize < _minimumRetained)
                        {
                            nextSize = _minimumRetained;
                        }

                        var removeCount = _objectCount - nextSize;
                        var removedCount = 0;
                        for (int i = 0; i < removeCount && !_disposedValue; i++)
                        {
                            if (_objctQueue.TryDequeue(out var item))
                            {
                                _lifecycleExecutor.Destroy(item);
                                removedCount++;
                                continue;
                            }
                            break;
                        }
                        if (removedCount > 0)
                        {
                            lock (_syncRoot)
                            {
                                _objectCount -= removedCount;
                            }
                        }
                    }
                }
            }, _cleanTaskCancellationTokenSource.Token);
        }

        #endregion Private 方法
    }
}