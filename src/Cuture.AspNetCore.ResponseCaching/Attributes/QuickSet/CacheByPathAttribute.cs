using System;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 根据请求路径进行响应缓存（不包含查询）
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class CacheByPathAttribute : ResponseCachingAttribute
{
    #region Public 构造函数

    /// <summary>
    /// 根据请求路径进行响应缓存（不包含查询）
    /// </summary>
    /// <param name="duration">缓存时长（秒）</param>
    public CacheByPathAttribute(int duration) : base(duration, CacheMode.PathUniqueness)
    {
    }

    /// <summary>
    /// 根据请求路径进行响应缓存（不包含查询）
    /// </summary>
    /// <param name="duration">缓存时长（秒）</param>
    /// <param name="storeLocation">缓存存储位置</param>
    public CacheByPathAttribute(int duration, CacheStoreLocation storeLocation) : this(duration)
    {
        StoreLocation = storeLocation;
    }

    #endregion Public 构造函数
}