using System;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 根据查询键进行响应缓存
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CacheByQueryAttribute : ResponseCachingAttribute
    {
        /// <summary>
        /// 根据查询键进行响应缓存
        /// </summary>
        /// <param name="duration">缓存时长（秒）</param>
        /// <param name="queryKeys">依据的具体查询键</param>
        public CacheByQueryAttribute(int duration, params string[] queryKeys) : base(duration)
        {
            if (queryKeys is null || queryKeys.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(queryKeys));
            }
            Duration = duration;
            VaryByQueryKeys = queryKeys;

            Mode = CacheMode.Custom;
        }
    }
}