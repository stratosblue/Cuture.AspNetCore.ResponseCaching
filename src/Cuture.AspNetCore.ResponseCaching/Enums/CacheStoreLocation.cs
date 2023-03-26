namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 缓存存储位置
/// </summary>
public enum CacheStoreLocation
{
    /// <summary>
    /// 默认（使用全局设置）
    /// </summary>
    Default,

    /// <summary>
    /// 分布式缓存
    /// </summary>
    Distributed,

    /// <summary>
    /// 内存
    /// </summary>
    Memory,
}
