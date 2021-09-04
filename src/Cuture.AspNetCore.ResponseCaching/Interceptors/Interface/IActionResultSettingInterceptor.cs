using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Interceptors
{
    /// <summary>
    /// <inheritdoc cref="IResponseCachingInterceptor"/> - 设置ActionResult
    /// </summary>
    public interface IActionResultSettingInterceptor : IResponseCachingInterceptor
    {
        #region Public 方法

        /// <summary>
        /// 设置ActionResult时拦截
        /// </summary>
        /// <param name="actionContext">Action执行上下文<see cref="ActionExecutingContext"/></param>
        /// <param name="actionResult"><see cref="IActionResult"/></param>
        /// <param name="next">后续进行将<see cref="IActionResult"/>设置到请求上下文的方法委托</param>
        /// <returns></returns>
        Task OnResultSettingAsync(ActionExecutingContext actionContext, IActionResult actionResult, OnResultSettingDelegate next);

        #endregion Public 方法
    }
}