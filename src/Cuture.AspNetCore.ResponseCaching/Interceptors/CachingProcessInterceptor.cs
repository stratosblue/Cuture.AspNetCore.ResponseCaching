using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Interceptors
{
    /// <inheritdoc cref="ICachingProcessInterceptor"/>
    public abstract class CachingProcessInterceptor : Attribute, ICachingProcessInterceptor
    {
        #region Public 方法

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SkipCall]
        public virtual Task<ResponseCacheEntry?> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate next)
        {
            return next(actionContext, key, entry);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SkipCall]
        public virtual Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next)
        {
            return next(actionContext, entry);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SkipCall]
        public virtual Task OnResultSettingAsync(ActionExecutingContext actionContext, IActionResult actionResult, OnResultSettingDelegate next)
        {
            return next(actionContext, actionResult);
        }

        #endregion Public 方法
    }
}