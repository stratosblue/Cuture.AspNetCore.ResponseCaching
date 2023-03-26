namespace Cuture.AspNetCore.ResponseCaching.Metadatas;

/// <summary>
/// <inheritdoc cref="IResponseCachingMetadata"/> - 最大可缓存响应长度
/// </summary>
public interface IMaxCacheableResponseLengthMetadata : IResponseCachingMetadata
{
    #region Public 属性

    /// <summary>
    /// 最大可缓存响应长度 (为 null 时，使用全局配置)
    /// </summary>
    int? MaxCacheableResponseLength { get; }

    #endregion Public 属性
}
