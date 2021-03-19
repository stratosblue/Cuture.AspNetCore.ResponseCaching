using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseCaching.Internal
{
    /// <summary>
    /// cacheKey独享锁的执行锁
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    internal abstract class ExclusiveExecutingLockerBase<TPayload> where TPayload : class
    {
        #region Private 字段

        /// <summary>
        /// 本地缓存可用毫秒数
        /// </summary>
        private readonly uint _localCacheAvailableMilliseconds;

        #endregion Private 字段

        #region Public 构造函数

        public ExclusiveExecutingLockerBase(IOptions<ResponseCachingOptions> options)
        {
            //TODO 支持差异化的 _localCacheAvailableMilliseconds
            _localCacheAvailableMilliseconds = options.Value.DefaultLocalCacheAvailableMilliseconds;
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <summary>
        /// 锁定运行
        /// </summary>
        /// <param name="lockState"></param>
        /// <param name="cacheAvailableFunc">缓存可用时的委托</param>
        /// <param name="cacheUnAvailableFunc">缓存不可用时的委托</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task LockRunAsync(ExecutionLockState<TPayload> lockState, Func<TPayload, Task> cacheAvailableFunc, Func<Task<TPayload?>> cacheUnAvailableFunc, CancellationToken token)
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
                if (lockState.LocalCachedPayload is not null
                    && lockState.LocalCachedPayloadExpireTime >= executingTime)
                {
                    await cacheAvailableFunc(lockState.LocalCachedPayload);
                    return;
                }

                var newPayload = await cacheUnAvailableFunc();

                if (newPayload != null)
                {
                    lockState.LocalCachedPayload = newPayload;
                    lockState.LocalCachedPayloadExpireTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + _localCacheAvailableMilliseconds;
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