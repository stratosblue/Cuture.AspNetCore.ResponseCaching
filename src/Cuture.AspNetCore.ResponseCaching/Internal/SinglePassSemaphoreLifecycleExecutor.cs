using Microsoft.Extensions.ObjectPool;

namespace Cuture.AspNetCore.ResponseCaching.Internal;

internal class SinglePassSemaphoreLifecycleExecutor : IObjectLifecycleExecutor<SemaphoreSlim>
{
    #region Public 方法

    public SemaphoreSlim Create() => new(1, 1);

    public void Destroy(SemaphoreSlim item) => item.Dispose();

    public bool Reset(SemaphoreSlim item)
    {
        while (item.CurrentCount < 1)
        {
            item.Release();
        }
        return true;
    }

    #endregion Public 方法
}
