namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 执行锁定模式
/// </summary>
public enum ExecutingLockMode
{
    /// <summary>
    /// 默认
    /// </summary>
    Default = 0,

    /// <summary>
    /// 没有控制，并发访问时可能穿过缓存
    /// </summary>
    None = 1,

    /// <summary>
    /// 根据Action放行单个请求
    /// </summary>
    ActionSingle = 2,

    /// <summary>
    /// 根据缓存键放行单个请求（可能会有很多的同步对象，慎重评估并使用此选项）
    /// </summary>
    CacheKeySingle = 3,
}