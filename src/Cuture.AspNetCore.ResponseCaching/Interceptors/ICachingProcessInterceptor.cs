using System;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Interceptors
{
    /// <summary>
    /// 缓存处理拦截器
    /// </summary>
    public interface ICachingProcessInterceptor
    {
        #region Public 方法

        /// <summary>
        /// 在缓存将进行存储时
        /// </summary>
        /// <param name="actionContext">Http请求方法的上下文<see cref="ActionContext"/></param>
        /// <param name="key">缓存key</param>
        /// <param name="entry">缓存项</param>
        /// <param name="storeFunc">进行缓存的方法委托</param>
        /// <returns></returns>
        Task<ResponseCacheEntry> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, Func<string, ResponseCacheEntry, Task<ResponseCacheEntry>> storeFunc);

        /// <summary>
        /// 在缓存将写入响应时
        /// </summary>
        /// <param name="actionContext">Http请求方法的上下文<see cref="ActionContext"/></param>
        /// <param name="entry">缓存项</param>
        /// <param name="writeFunc">进行将缓存写入响应的方法委托</param>
        /// <returns></returns>
        Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, Func<ActionContext, ResponseCacheEntry, Task<bool>> writeFunc);

        /// <summary>
        /// 在ActionFilter将直接设置响应结果时
        /// </summary>
        /// <param name="actionContext">Action执行上下文<see cref="ActionExecutingContext"/></param>
        /// <param name="actionResult"><see cref="IActionResult"/></param>
        /// <param name="setResultFunc">进行将<see cref="IActionResult"/>设置到请求上下文的方法委托</param>
        /// <returns></returns>
        Task OnResultSettingAsync(ActionExecutingContext actionContext, IActionResult actionResult, Func<ActionExecutingContext, IActionResult, Task> setResultFunc);

        #endregion Public 方法
    }
}