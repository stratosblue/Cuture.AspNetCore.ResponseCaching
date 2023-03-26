using System;

namespace Microsoft.Extensions.ObjectPool;

/// <summary>
/// 有界的对象池
/// </summary>
/// <typeparam name="T"></typeparam>
internal interface IBoundedObjectPool<T> : IDisposable
{
    #region Public 方法

    /// <summary>
    /// 当前可用对象数量
    /// </summary>
    int AvailableCount { get; }

    /// <summary>
    /// 池大小
    /// </summary>
    int PoolSize { get; }

    /// <summary>
    /// 从池中取一个对象
    /// <para/>
    /// 当没有可用对象时，返回 null
    /// </summary>
    /// <returns></returns>
    IObjectOwner<T>? Rent();

    #endregion Public 方法
}

/// <summary>
/// 直接借用对象的 <inheritdoc cref="IBoundedObjectPool{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
internal interface IDirectBoundedObjectPool<T> : IDisposable
{
    #region Public 方法

    /// <summary>
    /// 从池中取一个对象
    /// <para/>
    /// 当没有可用对象时，返回 null
    /// </summary>
    /// <returns></returns>
    T? Rent();

    #endregion Public 方法
}
