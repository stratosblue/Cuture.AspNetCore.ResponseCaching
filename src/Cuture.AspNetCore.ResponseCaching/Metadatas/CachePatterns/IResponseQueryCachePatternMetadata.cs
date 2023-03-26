namespace Cuture.AspNetCore.ResponseCaching.Metadatas;

/// <summary>
/// <inheritdoc cref="IResponseCachePatternMetadata"/> - 基于 Query 的缓存
/// </summary>
public interface IResponseQueryCachePatternMetadata : IResponseCachePatternMetadata
{
    #region Public 属性

    /// <summary>
    /// 创建缓存时依据的 Query 键
    /// </summary>
    string[]? VaryByQueryKeys { get; }

    #endregion Public 属性
}