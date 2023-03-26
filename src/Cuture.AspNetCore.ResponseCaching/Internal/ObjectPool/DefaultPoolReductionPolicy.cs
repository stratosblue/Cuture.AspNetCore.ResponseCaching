namespace Microsoft.Extensions.ObjectPool;

/// <summary>
/// 默认池缩减策略
/// </summary>
internal class DefaultPoolReductionPolicy : IPoolReductionPolicy
{
    #region Public 方法

    /// <inheritdoc/>
    public int NextSize(int currentPoolSize, int poolMinimumRetained)
    {
        var reserved = currentPoolSize - (currentPoolSize / 4);
        return reserved > poolMinimumRetained
                ? reserved
                : poolMinimumRetained;
    }

    #endregion Public 方法
}