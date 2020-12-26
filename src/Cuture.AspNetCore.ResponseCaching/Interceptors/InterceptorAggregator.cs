using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Interceptors
{
    /// <summary>
    /// 拦截器集合
    /// </summary>
    public class InterceptorAggregator : ICachingProcessInterceptor
    {
        #region Public 属性

        /// <inheritdoc cref="ICachingProcessInterceptor"/>
        public ICachingProcessInterceptor CachingProcessInterceptor { get; }

        #endregion Public 属性

        #region Public 构造函数

        public InterceptorAggregator(ICachingProcessInterceptor cachingProcessInterceptor)
        {
            CachingProcessInterceptor = cachingProcessInterceptor;
        }

        #endregion Public 构造函数

        #region ICachingProcessInterceptor

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<ResponseCacheEntry> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, int duration, Func<string, ResponseCacheEntry, int, Task<ResponseCacheEntry>> storeFunc)
        {
            if (CachingProcessInterceptor != null)
            {
                return CachingProcessInterceptor.OnCacheStoringAsync(actionContext, key, entry, duration, storeFunc);
            }
            return storeFunc(key, entry, duration);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, Func<ActionContext, ResponseCacheEntry, Task<bool>> writeFunc)
        {
            if (CachingProcessInterceptor != null)
            {
                return CachingProcessInterceptor.OnResponseWritingAsync(actionContext, entry, writeFunc);
            }

            return writeFunc(actionContext, entry);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task OnResultSettingAsync(ActionExecutingContext actionContext, IActionResult actionResult, Func<ActionExecutingContext, IActionResult, Task> setResultFunc)
        {
            if (CachingProcessInterceptor != null)
            {
                return CachingProcessInterceptor.OnResultSettingAsync(actionContext, actionResult, setResultFunc);
            }

            return setResultFunc(actionContext, actionResult);
        }

        #endregion ICachingProcessInterceptor
    }
}