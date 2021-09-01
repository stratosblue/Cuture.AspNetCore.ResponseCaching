using System;

using Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;

namespace Cuture.AspNetCore.ResponseCaching.Metadatas
{
    /// <summary>
    /// <inheritdoc cref="IResponseCachingMetadata"/> - 用于生成缓存Key的 <see cref="ICacheKeyGenerator"/> 实现类型
    /// </summary>
    public interface ICacheKeyGeneratorMetadata : IResponseCachingMetadata
    {
        #region Public 属性

        /// <summary>
        /// 用于生成缓存Key的 <see cref="ICacheKeyGenerator"/> 实现类型
        /// </summary>
        Type CacheKeyGeneratorType { get; }

        /// <summary>
        /// <see cref="CacheKeyGeneratorType"/> 对应的过滤器类型
        /// </summary>
        FilterType FilterType { get; }

        #endregion Public 属性
    }
}