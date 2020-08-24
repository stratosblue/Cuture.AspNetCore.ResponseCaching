using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Cuture.AspNetCore.ResponseCaching.Internal
{
    /// <summary>
    /// 可本地缓存锁
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class LocalCacheableLock<TPayload> : IDisposable where TPayload : class
    {
        /// <summary>
        /// 本地缓存可用毫秒数
        /// </summary>
        private readonly int _localCacheAvailableMilliseconds;

        /// <summary>
        /// 执行信号
        /// </summary>
        private readonly SemaphoreSlim _runNextSemaphore = new SemaphoreSlim(1, 1);

        private bool _disposedValue;

        /// <summary>
        /// 本地缓存数据
        /// </summary>
        private volatile TPayload _localCachedPayload;

        /// <summary>
        /// 本地缓存数据过期时间
        /// </summary>
        private long _localCachedPayloadExpireTime;

        /// <summary>
        /// 本地缓存可用毫秒数
        /// </summary>
        /// <param name="localCacheAvailableMilliseconds"></param>
        public LocalCacheableLock(int localCacheAvailableMilliseconds)
        {
            _localCacheAvailableMilliseconds = localCacheAvailableMilliseconds > 0 ? localCacheAvailableMilliseconds : throw new ArgumentOutOfRangeException($"{nameof(localCacheAvailableMilliseconds)} must be greater than 0");

            Debug.WriteLine($"{{0}}:new instance of {nameof(LocalCacheableLock<TPayload>)} created. localCacheAvailableMilliseconds:{{1}}", DateTime.Now, localCacheAvailableMilliseconds);
        }

        /// <summary>
        ///
        /// </summary>
        ~LocalCacheableLock()
        {
            Dispose(disposing: false);
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 锁定运行
        /// </summary>
        /// <param name="cacheAvailableFunc">缓存可用时的委托</param>
        /// <param name="cacheUnAvailableFunc">缓存不可用时的委托</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task LockRunAsync(Func<TPayload, Task> cacheAvailableFunc, Func<Task<TPayload>> cacheUnAvailableFunc, CancellationToken token)
        {
            try
            {
                //当前的执行时间
                var executingTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                await _runNextSemaphore.WaitAsync(token);
                if (token.IsCancellationRequested)
                {
                    return;
                }
                //有本地缓存，且当前执行时间小于等于本地缓存过期时间
                if (_localCachedPayload != null
                    && Interlocked.Read(ref _localCachedPayloadExpireTime) >= executingTime)
                {
                    await cacheAvailableFunc(_localCachedPayload);
                    return;
                }

                var newPayload = await cacheUnAvailableFunc();

                if (newPayload != null)
                {
                    _localCachedPayload = newPayload;
                    //设置本地缓存的过期时间
                    Interlocked.Exchange(ref _localCachedPayloadExpireTime, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + _localCacheAvailableMilliseconds);
                }
            }
            finally
            {
                _runNextSemaphore.Release();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                }

                _runNextSemaphore.Dispose();
                _localCachedPayload = null;

                _disposedValue = true;

                Debug.WriteLine($"{{0}}:{nameof(LocalCacheableLock<TPayload>)} Disposed.", DateTime.Now);
            }
        }
    }
}