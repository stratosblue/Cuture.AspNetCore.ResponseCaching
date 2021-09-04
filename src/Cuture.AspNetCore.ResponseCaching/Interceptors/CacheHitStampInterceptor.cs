using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Cuture.AspNetCore.ResponseCaching.Interceptors
{
    /// <summary>
    /// 缓存处理拦截器 - 缓存命中标记响应头
    /// </summary>
    internal class CacheHitStampInterceptor : IResponseWritingInterceptor, IActionResultSettingInterceptor
    {
        #region Private 字段

        private readonly string _headerKey;
        private readonly StringValues _headerValue;

        #endregion Private 字段

        #region Public 构造函数

        /// <summary>
        /// 缓存处理拦截器 - 缓存命中标记响应头
        /// </summary>
        /// <param name="headerKey"></param>
        /// <param name="headerValue"></param>
        public CacheHitStampInterceptor(string headerKey, StringValues headerValue)
        {
            if (string.IsNullOrEmpty(headerKey))
            {
                throw new ArgumentException($"“{nameof(headerKey)}”不能是 Null 或为空", nameof(headerKey));
            }

            if (string.IsNullOrEmpty(headerValue))
            {
                throw new ArgumentException($"“{nameof(headerValue)}”不能是 Null 或为空", nameof(headerValue));
            }

            _headerKey = headerKey;
            _headerValue = headerValue;
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> OnResponseWritingAsync(ActionContext actionContext,
                                                 ResponseCacheEntry entry,
                                                 OnResponseWritingDelegate next)
        {
            actionContext.HttpContext.Response.Headers.Add(_headerKey, _headerValue);
            return next(actionContext, entry);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task OnResultSettingAsync(ActionExecutingContext actionContext,
                                         IActionResult actionResult,
                                         OnResultSettingDelegate next)
        {
            actionContext.HttpContext.Response.Headers.Add(_headerKey, _headerValue);
            return next(actionContext, actionResult);
        }

        #endregion Public 方法
    }
}