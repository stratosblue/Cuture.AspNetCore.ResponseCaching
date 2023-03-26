using System;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 根据form表单键进行响应缓存
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class CacheByFormAttribute : ResponseCachingAttribute
{
    #region Public 构造函数

    /// <summary>
    /// 根据form表单键进行响应缓存
    /// </summary>
    /// <param name="duration">缓存时长（秒）</param>
    /// <param name="formKeys">依据的具体表单键</param>
    public CacheByFormAttribute(int duration, params string[] formKeys) : base(duration, CacheMode.Custom)
    {
        if (formKeys is null || formKeys.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(formKeys));
        }
        VaryByFormKeys = formKeys;
    }

    /// <summary>
    /// 根据form表单键进行响应缓存
    /// </summary>
    /// <param name="duration">缓存时长（秒）</param>
    /// <param name="storeLocation">缓存存储位置</param>
    /// <param name="formKeys">依据的具体表单键</param>
    public CacheByFormAttribute(int duration, CacheStoreLocation storeLocation, params string[] formKeys) : this(duration, formKeys)
    {
        StoreLocation = storeLocation;
    }

    #endregion Public 构造函数
}