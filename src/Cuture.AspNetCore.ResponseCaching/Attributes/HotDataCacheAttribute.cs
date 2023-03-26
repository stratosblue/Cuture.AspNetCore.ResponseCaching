using System;

using Cuture.AspNetCore.ResponseCaching.Metadatas;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 本地热数据缓存
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class HotDataCacheAttribute : Attribute, IHotDataCacheBuilder, IHotDataCacheMetadata
{
    #region Public 属性

    /// <inheritdoc/>
    public HotDataCachePolicy CachePolicy { get; } = HotDataCachePolicy.Default;

    /// <inheritdoc/>
    public int Capacity { get; }

    /// <inheritdoc/>
    public string? HotDataCacheName { get; }

    /// <inheritdoc cref="HotDataCacheAttribute(int, HotDataCachePolicy)"/>
    public HotDataCacheAttribute(int capacity)
    {
        Capacity = capacity;
    }

    /// <summary>
    /// <inheritdoc cref="HotDataCacheAttribute"/>
    /// </summary>
    /// <param name="capacity">缓存热数据的数量</param>
    /// <param name="cachePolicy">缓存替换策略</param>
    public HotDataCacheAttribute(int capacity, HotDataCachePolicy cachePolicy)
    {
        Capacity = capacity;
        CachePolicy = cachePolicy;
    }

    #endregion Public 属性

    #region Public 方法

    /// <inheritdoc/>
    public IHotDataCache Build(IServiceProvider serviceProvider, IHotDataCacheMetadata metadata)
    {
        var hotDataCacheProvider = serviceProvider.GetRequiredService<IHotDataCacheProvider>();
        return hotDataCacheProvider.Get(serviceProvider,
                                        metadata.HotDataCacheName ?? string.Empty,
                                        metadata.CachePolicy,
                                        metadata.Capacity);
    }

    #endregion Public 方法
}
