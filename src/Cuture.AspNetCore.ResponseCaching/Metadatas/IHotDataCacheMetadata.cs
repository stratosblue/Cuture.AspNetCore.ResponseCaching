using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

namespace Cuture.AspNetCore.ResponseCaching.Metadatas
{
    /// <summary>
    /// <inheritdoc cref="IResponseCachingMetadata"/> - <see cref="IHotDataCache"/> 信息
    /// </summary>
    public interface IHotDataCacheMetadata : IResponseCachingMetadata
    {
        #region Public 属性

        /// <summary>
        /// 热点数据缓存策略
        /// </summary>
        HotDataCachePolicy CachePolicy { get; }

        /// <summary>
        /// 缓存热数据的数量
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// 热点数据缓存容器名称
        /// </summary>
        string? HotDataCacheName { get; }

        #endregion Public 属性
    }
}