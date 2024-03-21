namespace Microsoft.Extensions.ObjectPool;

/// <summary>
/// 有限大小的对象池
/// </summary>
internal static class BoundedObjectPool
{
    #region Public 方法

    /// <summary>
    /// 使用 <see cref="DefaultObjectLifecycleExecutor{T}"/> 创建一个有限大小的对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="maximumPooled">最大对象数量</param>
    /// <param name="minimumRetained">最小保留对象数量</param>
    /// <param name="recycleIntervalSeconds">回收间隔(秒)</param>
    /// <returns></returns>
    public static IBoundedObjectPool<T> Create<T>(int maximumPooled, int minimumRetained, int recycleIntervalSeconds) where T : new()
    {
        return Create<T>(maximumPooled, minimumRetained, TimeSpan.FromSeconds(recycleIntervalSeconds));
    }

    /// <summary>
    /// 使用 <see cref="DefaultObjectLifecycleExecutor{T}"/> 创建一个有限大小的对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="maximumPooled">最大对象数量</param>
    /// <param name="minimumRetained">最小保留对象数量</param>
    /// <param name="recycleInterval">回收间隔</param>
    /// <returns></returns>
    public static IBoundedObjectPool<T> Create<T>(int maximumPooled, int minimumRetained, TimeSpan recycleInterval) where T : new()
    {
        var options = new BoundedObjectPoolOptions
        {
            MaximumPooled = maximumPooled,
            MinimumRetained = minimumRetained,
            RecycleInterval = recycleInterval,
        };
        return Create<T>(options);
    }

    /// <summary>
    /// 使用 <see cref="DefaultObjectLifecycleExecutor{T}"/> 创建一个有限大小的对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IBoundedObjectPool<T> Create<T>(BoundedObjectPoolOptions options) where T : new()
    {
        return Create(new DefaultObjectLifecycleExecutor<T>(), options);
    }

    /// <summary>
    /// 创建一个有限大小的对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objectLifecycleExecutor"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IBoundedObjectPool<T> Create<T>(IObjectLifecycleExecutor<T> objectLifecycleExecutor, BoundedObjectPoolOptions options)
    {
        return new DefaultBoundedObjectPool<T>(objectLifecycleExecutor, options);
    }

    #endregion Public 方法
}
