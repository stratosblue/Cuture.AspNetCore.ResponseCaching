using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseCaching.Metadatas;

/// <summary>
/// <inheritdoc cref="IResponseCachingMetadata"/> - 缓存键严格模式
/// </summary>
public interface ICacheKeyStrictModeMetadata : IResponseCachingMetadata
{
    #region Public 属性

    /// <summary>
    /// 缓存键严格模式（指定键找不到时的处理方式，为 <see cref="CacheKeyStrictMode.Default"/> 时，使用全局配置）
    /// </summary>
    CacheKeyStrictMode StrictMode { get; }

    #endregion Public 属性
}
