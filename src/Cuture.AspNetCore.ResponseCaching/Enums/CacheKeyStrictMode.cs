namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 缓存key的严格模式
/// </summary>
public enum CacheKeyStrictMode
{
    /// <summary>
    /// 默认
    /// </summary>
    Default = 0,

    /// <summary>
    /// 忽略不存在的Key
    /// </summary>
    Ignore = 1,

    /// <summary>
    /// 严格模式，key不存在时不进行缓存
    /// </summary>
    Strict = 2,

    /// <summary>
    /// 严格模式，并在key不存在时抛出异常
    /// </summary>
    StrictWithException = 10,
}
