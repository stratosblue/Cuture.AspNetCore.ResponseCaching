using System;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 根据请求头进行响应缓存
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class CacheByHeaderAttribute : ResponseCachingAttribute
{
    #region Public 构造函数

    /// <summary>
    /// 根据请求头进行响应缓存
    /// </summary>
    /// <param name="duration">缓存时长（秒）</param>
    /// <param name="headers">依据的请求头</param>
    public CacheByHeaderAttribute(int duration, params string[] headers) : base(duration, CacheMode.Custom)
    {
        if (headers is null || headers.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(headers));
        }
        VaryByHeaders = headers;
    }

    /// <summary>
    /// 根据请求头进行响应缓存
    /// </summary>
    /// <param name="duration">缓存时长（秒）</param>
    /// <param name="storeLocation">缓存存储位置</param>
    /// <param name="headers">依据的请求头</param>
    public CacheByHeaderAttribute(int duration, CacheStoreLocation storeLocation, params string[] headers) : this(duration, headers)
    {
        StoreLocation = storeLocation;
    }

    #endregion Public 构造函数
}
