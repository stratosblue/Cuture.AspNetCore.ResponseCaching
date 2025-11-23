using System.Diagnostics;

namespace Microsoft.Extensions.Caching.Memory;

/// <summary>
/// 内部使用的针对 LRU 的 LinkedListNode
/// <para/>
/// * 不会对参数进行任何合法检查
/// </summary>
/// <typeparam name="TValue"></typeparam>
[DebuggerDisplay("Current: {Value} , Previous: {Previous.Value} , Next: {Next.Value}")]
[method: DebuggerStepThrough]
internal class LRUSpecializedLinkedListNode<TValue>(in TValue value)
{
    #region Public 字段

    public LRUSpecializedLinkedListNode<TValue>? Next;

    public LRUSpecializedLinkedListNode<TValue>? Previous;

    public TValue Value = value;

    #endregion Public 字段
}
