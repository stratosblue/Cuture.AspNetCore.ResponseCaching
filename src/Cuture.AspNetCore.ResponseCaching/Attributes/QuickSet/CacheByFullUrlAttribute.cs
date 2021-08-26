using System;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 根据完整的请求url进行响应缓存（包含所有查询键）
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CacheByFullUrlAttribute : ResponseCachingAttribute
    {
        #region Public 构造函数

        /// <summary>
        /// 根据完整的请求url进行响应缓存（包含所有查询键）
        /// </summary>
        /// <param name="duration">缓存时长（秒）</param>
        public CacheByFullUrlAttribute(int duration) : base(duration, CacheMode.FullPathAndQuery)
        {
        }

        /// <summary>
        /// 根据完整的请求url进行响应缓存（包含所有查询键）
        /// </summary>
        /// <param name="duration">缓存时长（秒）</param>
        /// <param name="storeLocation">缓存存储位置</param>
        public CacheByFullUrlAttribute(int duration, CacheStoreLocation storeLocation) : this(duration)
        {
            StoreLocation = storeLocation;
        }

        #endregion Public 构造函数
    }
}