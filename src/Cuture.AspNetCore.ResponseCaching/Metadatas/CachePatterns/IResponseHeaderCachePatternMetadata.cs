namespace Cuture.AspNetCore.ResponseCaching.Metadatas;

/// <summary>
/// <inheritdoc cref="IResponseCachePatternMetadata"/> - 基于 Header 的缓存
/// </summary>
public interface IResponseHeaderCachePatternMetadata : IResponseCachePatternMetadata
{
    #region Public 属性

    /// <summary>
    /// 创建缓存时依据的 Header 键
    /// </summary>
    string[]? VaryByHeaders { get; }

    #endregion Public 属性
}
