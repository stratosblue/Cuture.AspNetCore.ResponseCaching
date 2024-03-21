namespace Cuture.AspNetCore.ResponseCaching;

/// <summary>
/// 缓存Key访问器
/// </summary>
public interface ICacheKeyAccessor
{
    #region Public 属性

    /// <summary>
    /// 缓存key
    /// </summary>
    string? Key { get; }

    #endregion Public 属性
}

internal sealed class CacheKeyAccessor : ICacheKeyAccessor
{
    #region Private 字段

    private static readonly AsyncLocal<string?> s_asyncLocalCacheKey = new();

    #endregion Private 字段

    #region Public 属性

    public string? Key { get => s_asyncLocalCacheKey.Value; set => s_asyncLocalCacheKey.Value = value; }

    #endregion Public 属性
}
