using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Interceptors
{
    /// <inheritdoc cref="ICachingProcessInterceptor"/>
    public abstract class CachingProcessInterceptor : ICachingProcessInterceptor
    {
        #region Public 方法

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual Task<ResponseCacheEntry> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, Func<string, ResponseCacheEntry, Task<ResponseCacheEntry>> storeFunc)
        {
            return storeFunc(key, entry);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, Func<ActionContext, ResponseCacheEntry, Task<bool>> writeFunc)
        {
            return writeFunc(actionContext, entry);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual Task OnResultSettingAsync(ActionExecutingContext actionContext, IActionResult actionResult, Func<ActionExecutingContext, IActionResult, Task> setResultFunc)
        {
            return setResultFunc(actionContext, actionResult);
        }

        #endregion Public 方法
    }
}