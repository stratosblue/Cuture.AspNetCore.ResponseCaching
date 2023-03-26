namespace Cuture.AspNetCore.ResponseCaching;

/// <summary>
/// 过滤器类型
/// </summary>
public enum FilterType
{
    /// <summary>
    /// ResourceFilter (在模型绑定前)
    /// </summary>
    Resource,

    /// <summary>
    /// ActionFilter (在模型绑定后)
    /// </summary>
    Action
}
