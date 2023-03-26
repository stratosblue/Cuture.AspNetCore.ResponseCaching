using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseCaching.Metadatas;

/// <summary>
/// <inheritdoc cref="IResponseCachingMetadata"/> - 缓存模式
/// </summary>
public interface ICacheModeMetadata : IResponseCachingMetadata
{
    #region Public 属性

    /// <summary>
    /// 缓存模式（设置依据什么内容进行缓存）
    /// </summary>
    CacheMode Mode { get; }

    #endregion Public 属性
}
