﻿using System.Threading.Tasks;

namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches
{
    /// <summary>
    /// 响应缓存
    /// </summary>
    public interface IResponseCache
    {
        #region Public 方法

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<ResponseCacheEntry?> GetAsync(string key);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entry"></param>
        /// <param name="duration">有效时长（秒）</param>
        /// <returns></returns>
        Task SetAsync(string key, ResponseCacheEntry entry, int duration);

        #endregion Public 方法
    }
}