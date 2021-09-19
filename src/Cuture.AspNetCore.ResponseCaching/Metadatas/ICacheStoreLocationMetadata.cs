using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseCaching.Metadatas
{
    /// <summary>
    /// <inheritdoc cref="IResponseCachingMetadata"/> - 缓存数据存储位置
    /// </summary>
    public interface ICacheStoreLocationMetadata : IResponseCachingMetadata
    {
        #region Public 属性

        /// <summary>
        /// 缓存数据存储位置（为 <see cref="CacheStoreLocation.Default"/> 时，使用全局配置）
        /// </summary>
        CacheStoreLocation StoreLocation { get; }

        #endregion Public 属性
    }
}