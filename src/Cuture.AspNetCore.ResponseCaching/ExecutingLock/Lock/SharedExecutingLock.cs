using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using Cuture.AspNetCore.ResponseCaching.Internal;

using Microsoft.Extensions.Caching.Memory;

namespace Cuture.AspNetCore.ResponseCaching;

internal sealed class SharedExecutingLock<TCachePayload>
    : ExecutingLockBase<TCachePayload>
    where TCachePayload : class
{
    #region Private 字段

    private readonly IMemoryCache _memoryCache;

    private string _key = string.Empty;

    #endregion Private 字段

    #region Public 构造函数

    public SharedExecutingLock(SemaphoreSlim semaphore, IMemoryCache memoryCache) : base(semaphore)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public override void SetLocalCache(string key, TCachePayload? payload, long expireMilliseconds)
    {
        Debug.WriteLine("{0} - SetLocalCache {1} {2} {3}", nameof(SharedExecutingLock<TCachePayload>), key, expireMilliseconds, payload);
        // HACK 此处使用私有变量 _key 存放当前的缓存key，在清除缓存时使用 _key 进行清除，逻辑上进行清除缓存时当前 lock 已经没有引用，理论上没有问题。。。
        if (payload is null)
        {
            _memoryCache.Remove(_key);
        }
        else
        {
            var localCachedPayload = new LocalCachedPayload<TCachePayload>
            {
                Payload = payload,
                ExpireTime = expireMilliseconds
            };
            _memoryCache.Set(key, localCachedPayload);
        }
        _key = key;
    }

    /// <inheritdoc/>
    public override bool TryGetLocalCache(string key, long checkMilliseconds, [NotNullWhen(true)] out TCachePayload? cachedPayload)
    {
        Debug.WriteLine("{0} - TryGetLocalCache {1} {2}", nameof(ExclusiveExecutingLock<TCachePayload>), key, checkMilliseconds);
        if (_memoryCache.TryGetValue<LocalCachedPayload<TCachePayload>>(key, out var localCachedPayload)
            && localCachedPayload.ExpireTime >= checkMilliseconds
            && localCachedPayload.Payload is not null)
        {
            cachedPayload = localCachedPayload.Payload;
            return true;
        }
        cachedPayload = null;
        return false;
    }

    #endregion Public 方法
}