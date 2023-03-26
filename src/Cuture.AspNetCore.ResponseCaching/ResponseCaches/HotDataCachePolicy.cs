namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches;

/// <summary>
/// 热数据缓存策略
/// </summary>
public enum HotDataCachePolicy
{
    /// <summary>
    /// 默认(当前只支持LRU)
    /// </summary>
    Default = 0,

    /// <summary>
    /// LRU
    /// </summary>
    LRU = 1,
}