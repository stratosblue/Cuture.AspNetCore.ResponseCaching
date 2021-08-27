using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseCaching.Internal
{
    /// <summary>
    /// 多cacheKey同锁的执行锁
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    internal abstract class SharedExecutingLockerBase<TPayload> where TPayload : class
    {
        #region Private 字段

        /// <summary>
        /// 本地缓存可用毫秒数
        /// </summary>
        private readonly uint _localCacheAvailableMilliseconds;

        private readonly IMemoryCache _memoryCache;

        #endregion Private 字段

        #region Public 构造函数

        public SharedExecutingLockerBase(IOptions<ResponseCachingOptions> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _memoryCache = options.Value.LockedExecutionLocalResultCache;

            //TODO 支持差异化的 _localCacheAvailableMilliseconds
            _localCacheAvailableMilliseconds = options.Value.DefaultLocalCacheAvailableMilliseconds;
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <summary>
        /// 锁定运行
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="lockState"></param>
        /// <param name="cacheAvailableFunc">缓存可用时的委托</param>
        /// <param name="cacheUnAvailableFunc">缓存不可用时的委托</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task LockRunAsync(string cacheKey, ExecutionLockState<TPayload> lockState, Func<TPayload, Task> cacheAvailableFunc, Func<Task<TPayload?>> cacheUnAvailableFunc, CancellationToken token)
        {
            try
            {
                //当前的执行时间
                var executingTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                await lockState.Lock.WaitAsync(token);
                if (token.IsCancellationRequested)
                {
                    return;
                }

                //有本地缓存，且当前执行时间小于等于本地缓存过期时间
                if (_memoryCache.TryGetValue<LocalCachedPayload<TPayload>>(cacheKey, out var localCachedPayload)
                    && localCachedPayload.ExpireTime >= executingTime)
                {
                    await cacheAvailableFunc(localCachedPayload.Payload);
                    return;
                }

                var newPayload = await cacheUnAvailableFunc();

                if (newPayload != null)
                {
                    localCachedPayload.Payload = newPayload;
                    localCachedPayload.ExpireTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + _localCacheAvailableMilliseconds;
                    _memoryCache.Set(cacheKey, localCachedPayload);
                }
            }
            finally
            {
                lockState.Lock.Release();
            }
        }

        #endregion Public 方法
    }
}