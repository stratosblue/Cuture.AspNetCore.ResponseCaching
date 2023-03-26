namespace Cuture.AspNetCore.ResponseCaching.Metadatas;

/// <summary>
/// <inheritdoc cref="IResponseCachingMetadata"/> - Dump响应的Stream初始容量
/// </summary>
public interface IDumpStreamInitialCapacityMetadata : IResponseCachingMetadata
{
    #region Public 属性

    /// <summary>
    /// Dump响应的Stream初始容量
    /// </summary>
    int Capacity { get; }

    #endregion Public 属性
}
