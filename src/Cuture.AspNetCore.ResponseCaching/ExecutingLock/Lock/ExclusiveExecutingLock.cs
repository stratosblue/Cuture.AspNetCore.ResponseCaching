using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Cuture.AspNetCore.ResponseCaching
{
    internal sealed class ExclusiveExecutingLock<TCachePayload>
        : ExecutingLockBase<TCachePayload>
        where TCachePayload : class
    {
        #region Private 字段

        /// <summary>
        /// 本地缓存数据
        /// </summary>
        private volatile TCachePayload? _localCachedPayload;

        /// <summary>
        /// 本地缓存数据过期时间
        /// </summary>
        private long _localCacheExpire;

        #endregion Private 字段

        #region Internal 构造函数

        internal ExclusiveExecutingLock(SemaphoreSlim semaphore) : base(semaphore)
        {
        }

        #endregion Internal 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public override void SetLocalCache(string key, TCachePayload? payload, long expireMilliseconds)
        {
            Debug.WriteLine("{0} - SetLocalCache {1} {2} {3}", nameof(ExclusiveExecutingLock<TCachePayload>), key, expireMilliseconds, payload);
            _localCachedPayload = payload;
            _localCacheExpire = expireMilliseconds;
        }

        /// <inheritdoc/>
        public override bool TryGetLocalCache(string key, long checkMilliseconds, [NotNullWhen(true)] out TCachePayload? cachedPayload)
        {
            Debug.WriteLine("{0} - TryGetLocalCache {1} {2}", nameof(ExclusiveExecutingLock<TCachePayload>), key, checkMilliseconds);
            if (checkMilliseconds <= _localCacheExpire
                && _localCachedPayload is not null)
            {
                cachedPayload = _localCachedPayload;
                return true;
            }

            cachedPayload = default;

            return false;
        }

        #endregion Public 方法
    }
}