using System.Collections;

namespace Microsoft.Extensions.Caching.Memory;

/// <summary>
/// 倒序枚举
/// </summary>
/// <typeparam name="T"></typeparam>
internal interface IReverseEnumerable<out T>
{
    #region Public 方法

    /// <summary>
    /// 获取倒序的枚举接口
    /// </summary>
    /// <returns></returns>
    IEnumerable<T> GetEnumerable();

    #endregion Public 方法
}

/// <summary>
/// 内部使用的针对 LRU 的 LinkedList
/// <para/>
/// * 线程不安全
/// <para/>
/// * 不会对参数进行任何合法检查
/// <para/>
/// * 内部认为所有操作都是逻辑正确的
/// </summary>
/// <typeparam name="TValue"></typeparam>
internal class LRUSpecializedLinkedList<TValue> : IEnumerable<TValue>, IReverseEnumerable<TValue>, IEnumerable<LRUSpecializedLinkedListNode<TValue>>, IReverseEnumerable<LRUSpecializedLinkedListNode<TValue>>
{
    #region Private 字段

    private readonly int _capacity;
    private int _count;
    private LRUSpecializedLinkedListNode<TValue>? _head;
    private LRUSpecializedLinkedListNode<TValue>? _tail;

    #endregion Private 字段

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="LRUSpecializedLinkedList{TValue}"/>
    /// </summary>
    /// <param name="capacity">容量限制</param>
    public LRUSpecializedLinkedList(int capacity)
    {
        _capacity = capacity;
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <summary>
    /// 插入元素到头部
    /// <para/>
    /// * 默认为节点没有添加到列表，并且待添加的节点 <see cref="LRUSpecializedLinkedListNode{TValue}.Previous"/> 和 <see cref="LRUSpecializedLinkedListNode{TValue}.Next"/> 都为 null
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public LRUSpecializedLinkedListNode<TValue>? InsertAtHead(LRUSpecializedLinkedListNode<TValue> node)
    {
        if (_count > 1)
        {
            _head!.Previous = node;
            node.Next = _head;
            _head = node;

            if (_count >= _capacity)
            {
                return RemoveTail();
            }
            _count++;
        }
        else if (_count == 1)
        {
            _count = 2;
            _tail = _head;
            _head = node;
            _head.Next = _tail;
            _tail!.Previous = _head;
        }
        else
        {
            _count = 1;
            _head = node;
        }

        return null;
    }

    /// <summary>
    /// 移动元素到头部
    /// <para/>
    /// * 默认为节点已经通过 <see cref="InsertAtHead(LRUSpecializedLinkedListNode{TValue})"/> 添加到列表
    /// </summary>
    /// <param name="node"></param>
    public void MoveToHead(LRUSpecializedLinkedListNode<TValue> node)
    {
        if (node == _head)
        {
            return;
        }

        if (node == _tail)
        {
            _tail = _tail.Previous;
            _tail!.Next = null;
            _head!.Previous = node;
            node.Next = _head;
            node.Previous = null;
            _head = node;
            return;
        }

        node.Previous!.Next = node.Next;
        node.Next!.Previous = node.Previous;

        _head!.Previous = node;
        node.Next = _head;
        node.Previous = null;
        _head = node;
    }

    /// <summary>
    /// 移除节点
    /// <para/>
    /// * 默认为节点已经通过 <see cref="InsertAtHead(LRUSpecializedLinkedListNode{TValue})"/> 添加到列表
    /// </summary>
    /// <param name="node"></param>
    public void Remove(LRUSpecializedLinkedListNode<TValue> node)
    {
        if (node.Previous is not null) //移除的不是首节点
        {
            node.Previous.Next = node.Next;
            if (node.Next is not null)  //移除的不是尾节点
            {
                node.Next.Previous = node.Previous;
            }
            else    //移除的是尾节点
            {
                _tail = node.Previous;
                _tail.Next = null;
            }
        }
        else    //移除的是首节点
        {
            _head = node.Next;
            if (_head is not null)
            {
                _head.Previous = null;
            }
        }

        if (--_count == 1)
        {
            _tail = null;
        }
    }

    #region IEnumerable

    #region TValue

    /// <inheritdoc/>
    IEnumerable<TValue> IReverseEnumerable<TValue>.GetEnumerable()
    {
        if (_count == 1)
        {
            yield return _head!.Value;
        }
        else
        {
            var node = _tail;
            while (node != null)
            {
                yield return node.Value;
                node = node.Previous;
            }
        }
    }

    /// <inheritdoc/>
    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
    {
        var node = _head;
        while (node != null)
        {
            yield return node.Value;
            node = node.Next;
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return (this as IEnumerable<TValue>).GetEnumerator();
    }

    #endregion TValue

    #region LRUSpecializedLinkedListNode

    /// <inheritdoc/>
    IEnumerable<LRUSpecializedLinkedListNode<TValue>> IReverseEnumerable<LRUSpecializedLinkedListNode<TValue>>.GetEnumerable()
    {
        if (_count == 1)
        {
            yield return _head!;
        }
        else
        {
            var node = _tail;
            while (node != null)
            {
                yield return node;
                node = node.Previous;
            }
        }
    }

    /// <inheritdoc/>
    IEnumerator<LRUSpecializedLinkedListNode<TValue>> IEnumerable<LRUSpecializedLinkedListNode<TValue>>.GetEnumerator()
    {
        var node = _head;
        while (node != null)
        {
            yield return node;
            node = node.Next;
        }
    }

    #endregion LRUSpecializedLinkedListNode

    #endregion IEnumerable

    #endregion Public 方法

    #region Private 方法

    private LRUSpecializedLinkedListNode<TValue>? RemoveTail()
    {
        var removedNode = _tail;
        _tail = _tail!.Previous;
        _tail!.Next = null;

        return removedNode;
    }

    #endregion Private 方法
}
