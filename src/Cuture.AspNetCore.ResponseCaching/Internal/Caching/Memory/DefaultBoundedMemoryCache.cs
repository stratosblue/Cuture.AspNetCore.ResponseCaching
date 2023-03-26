using System;

namespace Microsoft.Extensions.Caching.Memory;

/// <inheritdoc cref="IBoundedMemoryCache{TKey, TValue}"/>
internal class DefaultBoundedMemoryCache<TKey, TValue> : IBoundedMemoryCache<TKey, TValue> where TValue : class
{
    #region Private 字段

    private readonly IBoundedMemoryCachePolicy<TKey, TValue> _cachePolicy;

    #endregion Private 字段

    #region Public 构造函数

    /// <inheritdoc cref="DefaultBoundedMemoryCache{TKey, TValue}"/>
    public DefaultBoundedMemoryCache(IBoundedMemoryCachePolicy<TKey, TValue> cachePolicy)
    {
        _cachePolicy = cachePolicy ?? throw new ArgumentNullException(nameof(cachePolicy));
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public void Add(in BoundedMemoryCacheEntry<TKey, TValue> cacheEntry) => _cachePolicy.Add(cacheEntry);

    /// <inheritdoc/>
    public bool Remove(TKey key) => _cachePolicy.Remove(key);

    /// <inheritdoc/>
    public bool TryGet(TKey key, out TValue? item) => _cachePolicy.TryGet(key, out item);

    #endregion Public 方法
}
