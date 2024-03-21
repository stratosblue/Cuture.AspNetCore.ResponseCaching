namespace Cuture.AspNetCore.ResponseCaching;

/// <summary>
/// <see cref="IExecutingLock{TCachePayload}"/> 池
/// </summary>
public interface IExecutingLockPool<TCachePayload>
    : IDisposable
    where TCachePayload : class
{
    #region Public 方法

    /// <summary>
    /// 通过 <paramref name="lockKey"/> 获取一个对应的 <see cref="IExecutingLock{TCachePayload}"/>
    /// </summary>
    /// <param name="lockKey"></param>
    /// <returns><paramref name="lockKey"/>对应的<see cref="IExecutingLock{TCachePayload}"/>，如果池已用尽，则返回 null</returns>
    IExecutingLock<TCachePayload>? GetLock(string lockKey);

    /// <summary>
    /// 将一个 <see cref="IExecutingLock{TCachePayload}"/> 归还给池
    /// </summary>
    /// <param name="item"></param>
    void Return(IExecutingLock<TCachePayload> item);

    #endregion Public 方法
}
