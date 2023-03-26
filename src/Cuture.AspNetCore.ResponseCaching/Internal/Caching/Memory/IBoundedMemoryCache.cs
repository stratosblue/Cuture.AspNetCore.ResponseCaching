namespace Microsoft.Extensions.Caching.Memory;

/// <summary>
/// 有限大小的内存缓存
/// </summary>
/// <typeparam name="TKey">缓存键</typeparam>
/// <typeparam name="TValue">缓存值</typeparam>
internal interface IBoundedMemoryCache<TKey, TValue> where TValue : class
{
    #region Public 方法

    /// <summary>
    /// 添加缓存项
    /// </summary>
    /// <param name="cacheEntry"></param>
    void Add(in BoundedMemoryCacheEntry<TKey, TValue> cacheEntry);

    /// <summary>
    /// 移除缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Remove(TKey key);

    /// <summary>
    /// 尝试获取缓存值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    bool TryGet(TKey key, out TValue? item);

    #endregion Public 方法
}