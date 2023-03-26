using System;

namespace Microsoft.Extensions.Caching.Memory;

/// <summary>
/// 缓存项被移除时的回调
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <param name="key"></param>
/// <param name="value"></param>
internal delegate void CacheEntryRemovingCallback<TKey, TValue>(TKey key, TValue value) where TValue : class;

/// <summary>
/// 有限大小的内存缓存项
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
internal readonly struct BoundedMemoryCacheEntry<TKey, TValue> where TValue : class
{
    #region Public 字段

    /// <summary>
    /// 移除缓存项时的回调
    /// </summary>
    public readonly CacheEntryRemovingCallback<TKey, TValue>? EntryRemovingCallback;

    /// <summary>
    /// 缓存Key
    /// </summary>
    public readonly TKey Key;

    /// <summary>
    /// 缓存值
    /// </summary>
    public readonly TValue Value;

    #endregion Public 字段

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="BoundedMemoryCacheEntry{TKey, TValue}"/>
    /// </summary>
    /// <param name="key">缓存Key</param>
    /// <param name="value">缓存值</param>
    /// <param name="entryRemovingCallback">移除缓存项时的回调</param>
    public BoundedMemoryCacheEntry(TKey key, TValue value, CacheEntryRemovingCallback<TKey, TValue>? entryRemovingCallback)
    {
        Key = key;
        Value = value ?? throw new ArgumentNullException(nameof(value));
        EntryRemovingCallback = entryRemovingCallback;
    }

    #endregion Public 构造函数
}
