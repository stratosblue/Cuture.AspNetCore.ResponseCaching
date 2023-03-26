namespace Microsoft.Extensions.ObjectPool;

/// <summary>
/// 回收池
/// </summary>
/// <typeparam name="T"></typeparam>
internal interface IRecyclePool<T>
{
    #region Public 方法

    /// <summary>
    /// 将对象还回对象池
    /// </summary>
    /// <param name="item"></param>
    void Return(T item);

    #endregion Public 方法
}