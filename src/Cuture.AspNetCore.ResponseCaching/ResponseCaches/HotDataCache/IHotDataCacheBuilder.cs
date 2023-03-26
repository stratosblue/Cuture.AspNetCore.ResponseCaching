using System;

using Cuture.AspNetCore.ResponseCaching.Metadatas;

namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches;

/// <summary>
/// 热点数据缓存提供器
/// </summary>
public interface IHotDataCacheBuilder
{
    #region Public 方法

    /// <summary>
    /// 获取热点数据缓存
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="metadata">热点数据缓存信息</param>
    /// <returns></returns>
    IHotDataCache Build(IServiceProvider serviceProvider, IHotDataCacheMetadata metadata);

    #endregion Public 方法
}
