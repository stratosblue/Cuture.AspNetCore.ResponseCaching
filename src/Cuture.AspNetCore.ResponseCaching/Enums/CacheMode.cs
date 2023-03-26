namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 缓存模式
/// </summary>
public enum CacheMode
{
    /// <summary>
    /// 默认
    /// </summary>
    Default = FullPathAndQuery,

    /// <summary>
    /// 完整的请求路径和查询键
    /// </summary>
    FullPathAndQuery = 1,

    /// <summary>
    /// 自定义
    /// </summary>
    Custom = 2,

    /// <summary>
    /// 请求路径唯一缓存（不包含查询）
    /// </summary>
    PathUniqueness = 3,
}
