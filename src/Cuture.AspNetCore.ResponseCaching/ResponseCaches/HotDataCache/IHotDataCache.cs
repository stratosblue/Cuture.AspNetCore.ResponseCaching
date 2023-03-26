﻿using System;

namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches;

/// <summary>
/// 热点数据缓存
/// </summary>
public interface IHotDataCache : IDisposable
{
    #region Public 方法

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    ResponseCacheEntry? Get(string key);

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    void Set(string key, ResponseCacheEntry entry);

    #endregion Public 方法
}
