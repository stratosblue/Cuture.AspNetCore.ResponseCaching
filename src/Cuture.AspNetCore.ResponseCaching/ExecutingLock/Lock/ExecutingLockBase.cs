using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cuture.AspNetCore.ResponseCaching;

[DebuggerDisplay("ReferenceCount = {ReferenceCount} , LockKey = {LockKey}")]
internal abstract class ExecutingLockBase<TCachePayload>
    : IExecutingLock<TCachePayload>
    where TCachePayload : class
{
    #region Public 字段

    /// <summary>
    /// 引用计数
    /// </summary>
    public int ReferenceCount = 0;

    public SemaphoreSlim Semaphore;

    /// <summary>
    /// 锁的唯一标识
    /// </summary>
    public string? LockKey { get; set; }

    #endregion Public 字段

    #region Internal 构造函数

    internal ExecutingLockBase(SemaphoreSlim semaphore)
    {
        Semaphore = semaphore;
    }

    #endregion Internal 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public int Release()
    {
        return Semaphore.Release();
    }

    /// <inheritdoc/>
    public abstract void SetLocalCache(string key, TCachePayload? payload, long expireMilliseconds);

    /// <inheritdoc/>
    public abstract bool TryGetLocalCache(string key, long checkMilliseconds, [NotNullWhen(true)] out TCachePayload? cachedPayload);

    /// <inheritdoc/>
    public Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
    {
        return Semaphore.WaitAsync(millisecondsTimeout, cancellationToken);
    }

    #endregion Public 方法
}