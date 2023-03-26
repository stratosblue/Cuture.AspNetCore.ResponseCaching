using System;

namespace Microsoft.Extensions.ObjectPool;

/// <summary>
/// 默认对象生命周期执行器
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class DefaultObjectLifecycleExecutor<T> : IObjectLifecycleExecutor<T> where T : new()
{
    #region Public 方法

    /// <inheritdoc/>
    public T? Create() => new();

    /// <inheritdoc/>
    public void Destroy(T item)
    {
        if (item is IDisposable disposable)
        {
            disposable.Dispose();
        }
        else if (item is IAsyncDisposable asyncDisposable)
        {
            _ = asyncDisposable.DisposeAsync();
        }
    }

    /// <inheritdoc/>
    public bool Reset(T item) => true;

    #endregion Public 方法
}