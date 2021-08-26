using System;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 依据Claim声明进行响应缓存
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CacheByClaimAttribute : ResponseCachingAttribute
    {
        #region Public 构造函数

        /// <summary>
        /// 依据Claim声明进行响应缓存
        /// </summary>
        /// <param name="duration">缓存时长（秒）</param>
        /// <param name="claimTypes">依据的具体ClaimType</param>
        public CacheByClaimAttribute(int duration, params string[] claimTypes) : base(duration, CacheMode.Custom)
        {
            if (claimTypes is null || claimTypes.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(claimTypes));
            }
            VaryByClaims = claimTypes;
        }

        /// <summary>
        /// 依据Claim声明进行响应缓存
        /// </summary>
        /// <param name="duration">缓存时长（秒）</param>
        /// <param name="storeLocation">缓存存储位置</param>
        /// <param name="claimTypes">依据的具体ClaimType</param>
        public CacheByClaimAttribute(int duration, CacheStoreLocation storeLocation, params string[] claimTypes) : this(duration, claimTypes)
        {
            StoreLocation = storeLocation;
        }

        #endregion Public 构造函数
    }
}