namespace Cuture.AspNetCore.ResponseCaching.Metadatas
{
    /// <summary>
    /// <inheritdoc cref="IResponseCachePatternMetadata"/> - 基于 Claim 的缓存
    /// </summary>
    public interface IResponseClaimCachePatternMetadata : IResponseCachePatternMetadata
    {
        #region Public 属性

        /// <summary>
        /// 创建缓存时依据的 Claim 类型
        /// </summary>
        string[]? VaryByClaims { get; }

        #endregion Public 属性
    }
}