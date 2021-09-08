using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseCaching.Interceptors
{
    /// <summary>
    /// <inheritdoc cref="IResponseCachingInterceptor"/> - 缓存存储
    /// </summary>
    public interface ICacheStoringInterceptor : IResponseCachingInterceptor
    {
        #region Public 方法

        /// <summary>
        /// 缓存存储时拦截
        /// </summary>
        /// <param name="actionContext">Http请求方法的上下文<see cref="ActionContext"/></param>
        /// <param name="key">缓存key</param>
        /// <param name="entry">缓存项</param>
        /// <param name="next">后续进行缓存的方法委托</param>
        /// <returns>进行内存缓存，以用于立即响应的 <see cref="ResponseCacheEntry"/></returns>
        Task<ResponseCacheEntry?> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate<ActionContext> next);

        #endregion Public 方法
    }
}