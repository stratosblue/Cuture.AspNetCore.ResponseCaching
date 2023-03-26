namespace Microsoft.Extensions.ObjectPool;

/// <summary>
/// 可直接借用、归还的对象池，不使用<see cref="IObjectOwner{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
internal interface INakedBoundedObjectPool<T> : IRecyclePool<T>, IDirectBoundedObjectPool<T>
{
}
