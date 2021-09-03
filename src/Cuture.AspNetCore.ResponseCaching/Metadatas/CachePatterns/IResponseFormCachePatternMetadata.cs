namespace Cuture.AspNetCore.ResponseCaching.Metadatas
{
    /// <summary>
    /// <inheritdoc cref="IResponseCachePatternMetadata"/> - 基于 Form 的缓存
    /// </summary>
    public interface IResponseFormCachePatternMetadata : IResponseCachePatternMetadata
    {
        #region Public 属性

        /// <summary>
        /// 创建缓存时依据的 Form 键
        /// </summary>
        string[]? VaryByFormKeys { get; }

        #endregion Public 属性
    }
}