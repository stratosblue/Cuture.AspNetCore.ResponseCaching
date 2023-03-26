namespace Microsoft.Extensions.Caching.Memory;

/// <summary>
/// 有限大小的内存缓存
/// </summary>
internal static class BoundedMemoryCache
{
    #region Public 方法

    /// <summary>
    /// 使用指定策略，创建一个内存缓存
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="cachePolicy"></param>
    /// <returns></returns>
    public static IBoundedMemoryCache<TKey, TValue> Create<TKey, TValue>(IBoundedMemoryCachePolicy<TKey, TValue> cachePolicy) where TValue : class
    {
        return new DefaultBoundedMemoryCache<TKey, TValue>(cachePolicy);
    }

    /// <summary>
    /// 使用 LRU 算法，创建一个最大容量为 <paramref name="capacity"/> 的内存缓存
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="capacity">最大容量</param>
    /// <returns></returns>
    public static IBoundedMemoryCache<TKey, TValue> CreateLRU<TKey, TValue>(int capacity) where TKey : notnull where TValue : class
    {
        return Create(new LRUMemoryCachePolicy<TKey, TValue>(capacity));
    }

    #endregion Public 方法
}
