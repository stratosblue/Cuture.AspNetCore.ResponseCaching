using System;

namespace Microsoft.Extensions.ObjectPool;

/// <summary>
/// 有限大小的对象池配置项
/// </summary>
internal class BoundedObjectPoolOptions
{
    #region Public 属性

    /// <summary>
    /// 最大池对象数量
    /// </summary>
    public int MaximumPooled { get; set; }

    /// <summary>
    /// 回收时，池中保留的最小对象数量
    /// </summary>
    public int MinimumRetained { get; set; }

    /// <inheritdoc cref="IPoolReductionPolicy"/>
    public IPoolReductionPolicy? PoolReductionPolicy { get; set; }

    /// <summary>
    /// 自动回收对象的检查间隔
    /// </summary>
    public TimeSpan RecycleInterval { get; set; } = TimeSpan.FromSeconds(120);

    #endregion Public 属性
}