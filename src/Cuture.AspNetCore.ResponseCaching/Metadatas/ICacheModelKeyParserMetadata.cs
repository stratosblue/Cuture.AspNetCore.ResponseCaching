namespace Cuture.AspNetCore.ResponseCaching.Metadatas;

/// <summary>
/// <inheritdoc cref="IResponseCachingMetadata"/> - 用于生成Model的缓存Key的 <see cref="IModelKeyParser"/> 实现类型
/// </summary>
public interface ICacheModelKeyParserMetadata : IResponseCachingMetadata
{
    #region Public 属性

    /// <summary>
    /// 用于生成Model的缓存Key的 <see cref="IModelKeyParser"/> 实现类型
    /// </summary>
    Type ModelKeyParserType { get; }

    #endregion Public 属性
}
