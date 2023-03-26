namespace Microsoft.Extensions.ObjectPool;

/// <summary>
/// 对象生命周期执行器
/// </summary>
/// <typeparam name="T"></typeparam>
internal interface IObjectLifecycleExecutor<T>
{
    #region Public 方法

    /// <summary>
    /// 创建对象
    /// </summary>
    /// <returns></returns>
    T? Create();

    /// <summary>
    /// 销毁对象
    /// </summary>
    /// <param name="item"></param>
    void Destroy(T item);

    /// <summary>
    /// 重置对象
    /// </summary>
    /// <param name="item"></param>
    /// <returns>是否重置成功</returns>
    bool Reset(T item);

    #endregion Public 方法
}
