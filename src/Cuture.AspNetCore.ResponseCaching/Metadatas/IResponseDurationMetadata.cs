namespace Cuture.AspNetCore.ResponseCaching.Metadatas;

/// <summary>
/// <inheritdoc cref="IResponseCachingMetadata"/> - 缓存时长
/// </summary>
public interface IResponseDurationMetadata : IResponseCachingMetadata
{
    #region Public 属性

    /// <summary>
    /// 缓存时长（单位：秒）
    /// </summary>
    int Duration { get; }

    #endregion Public 属性
}
