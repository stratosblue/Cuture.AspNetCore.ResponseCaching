using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.ObjectPool;

/// <summary>
/// 对象所有者
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class ObjectOwner<T> : IObjectOwner<T>
{
    #region Private 字段

    private readonly IRecyclePool<T> _recyclePool;
    private T _item;

    #endregion Private 字段

    #region Public 属性

    /// <inheritdoc/>
    public T Item { get => _item ?? throw new ObjectDisposedException(nameof(IObjectOwner<T>)); }

    #endregion Public 属性

    #region Public 构造函数

    public ObjectOwner(IRecyclePool<T> recyclePool, T item)
    {
        _recyclePool = recyclePool ?? throw new ArgumentNullException(nameof(recyclePool));
        _item = item;
    }

    #endregion Public 构造函数

    #region Private 析构函数

    ~ObjectOwner()
    {
        DoDispose();
    }

    #endregion Private 析构函数

    #region Public 方法

    /// <inheritdoc/>
    public void Dispose()
    {
        DoDispose();
        GC.SuppressFinalize(this);
    }

    #endregion Public 方法

    #region Private 方法

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DoDispose()
    {
        var item = _item;
        if (item != null)
        {
            _item = default!;
            _recyclePool.Return(item);
        }
    }

    #endregion Private 方法
}