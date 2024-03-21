namespace Microsoft.Extensions.Caching.Memory;

/// <summary>
/// LRU缓存策略
/// </summary>
/// <typeparam name="TKey">缓存键</typeparam>
/// <typeparam name="TValue">缓存值</typeparam>
internal class LRUMemoryCachePolicy<TKey, TValue> : IBoundedMemoryCachePolicy<TKey, TValue> where TKey : notnull where TValue : class
{
    #region Private 字段

    private readonly Dictionary<TKey, LRUSpecializedLinkedListNode<BoundedMemoryCacheEntry<TKey, TValue>>> _entries;
    private readonly LRUSpecializedLinkedList<BoundedMemoryCacheEntry<TKey, TValue>> _linkedList;
    private readonly object _syncRoot = new();

    #endregion Private 字段

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="LRUMemoryCachePolicy{TKey, TValue}"/>
    /// </summary>
    /// <param name="capacity">最大容量</param>
    public LRUMemoryCachePolicy(int capacity)
    {
        if (capacity < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "最大容量不能小于 2 ");
        }

        _linkedList = new(capacity);
        _entries = new(capacity + capacity / 2);
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public void Add(in BoundedMemoryCacheEntry<TKey, TValue> cacheEntry)
    {
        LRUSpecializedLinkedListNode<BoundedMemoryCacheEntry<TKey, TValue>>? removedNode = null;

        // lock 简单、方便、快捷
        lock (_syncRoot)
        {
            if (_entries.TryGetValue(cacheEntry.Key, out var node))
            {
                node.Value = cacheEntry;
                _linkedList.MoveToHead(node);
                return;
            }
            else
            {
                node = new(cacheEntry);
                _entries.Add(cacheEntry.Key, node);
            }

            removedNode = _linkedList.InsertAtHead(node);

            if (removedNode != null)
            {
                _entries.Remove(removedNode.Value.Key);
            }
        }

        TryFireRemovingCallback(removedNode);
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        lock (_syncRoot)
        {
            if (_entries.Remove(key, out var node))
            {
                _linkedList.Remove(node);
                TryFireRemovingCallback(node);
                return true;
            }
        }
        return false;
    }

    /// <inheritdoc/>
    public bool TryGet(TKey key, out TValue? item)
    {
        lock (_syncRoot)
        {
            if (_entries.TryGetValue(key, out var node))
            {
                _linkedList.MoveToHead(node);
                item = node.Value.Value;
                return true;
            }
        }

        item = default;
        return false;
    }

    #endregion Public 方法

    #region Private 方法

    private static void TryFireRemovingCallback(LRUSpecializedLinkedListNode<BoundedMemoryCacheEntry<TKey, TValue>>? removedNode)
    {
        if (removedNode?.Value.EntryRemovingCallback is CacheEntryRemovingCallback<TKey, TValue> removingCallback)
        {
            Task.Run(() =>
            {
                removingCallback(removedNode.Value.Key, removedNode.Value.Value);
            });
        }
    }

    #endregion Private 方法
}
