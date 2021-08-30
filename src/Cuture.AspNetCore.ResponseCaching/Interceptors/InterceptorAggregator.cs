using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Interceptors
{
    /// <summary>
    /// 委托 - <inheritdoc cref="ICachingProcessInterceptor.OnCacheStoringAsync(ActionContext, string, ResponseCacheEntry, OnCacheStoringDelegate)"/>
    /// </summary>
    /// <param name="actionContext"></param>
    /// <param name="key"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public delegate Task<ResponseCacheEntry> OnCacheStoringDelegate(ActionContext actionContext, string key, ResponseCacheEntry entry);

    /// <summary>
    /// 委托 - <inheritdoc cref="ICachingProcessInterceptor.OnResponseWritingAsync(ActionContext, ResponseCacheEntry, OnResponseWritingDelegate)"/>
    /// </summary>
    /// <param name="actionContext"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public delegate Task<bool> OnResponseWritingDelegate(ActionContext actionContext, ResponseCacheEntry entry);

    /// <summary>
    /// 委托 - <inheritdoc cref="ICachingProcessInterceptor.OnResultSettingAsync(ActionExecutingContext, IActionResult, OnResultSettingDelegate)"/>
    /// </summary>
    /// <param name="actionContext"></param>
    /// <param name="actionResult"></param>
    /// <returns></returns>
    public delegate Task OnResultSettingDelegate(ActionExecutingContext actionContext, IActionResult actionResult);

    /// <summary>
    /// 拦截器集合
    /// </summary>
    public class InterceptorAggregator
    {
        #region Public 委托

        /// <summary>
        ///
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="key"></param>
        /// <param name="entry"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public delegate Task<ResponseCacheEntry> OnCacheStoringWrappedDelegate(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate next);

        /// <summary>
        ///
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="entry"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public delegate Task<bool> OnResponseWritingWrappedDelegate(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next);

        /// <summary>
        ///
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="actionResult"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public delegate Task OnResultSettingWrappedDelegate(ActionExecutingContext actionContext, IActionResult actionResult, OnResultSettingDelegate next);

        #endregion Public 委托

        #region Private 字段

        private readonly IEnumerable<ICachingProcessInterceptor>? _interceptors;

        private readonly OnCacheStoringWrappedDelegate _onCacheStoringWrappedDelegate;
        private readonly OnResponseWritingWrappedDelegate _onResponseWritingWrappedDelegate;
        private readonly OnResultSettingWrappedDelegate _onResultSettingWrappedDelegate;

        #endregion Private 字段

        #region Public 构造函数

        /// <summary>
        /// 拦截器集合
        /// </summary>
        /// <param name="interceptors"><see cref="ICachingProcessInterceptor"/></param>
        public InterceptorAggregator(IEnumerable<ICachingProcessInterceptor>? interceptors)
        {
            _onCacheStoringWrappedDelegate = new(static (actionContext, key, entry, next) => next(actionContext, key, entry));
            _onResponseWritingWrappedDelegate = new(static (actionContext, entry, next) => next(actionContext, entry));
            _onResultSettingWrappedDelegate = new(static (actionContext, actionResult, next) => next(actionContext, actionResult));

            if (interceptors is not null)
            {
                var reversedInterceptors = interceptors.Reverse().ToArray();

                for (int i = 0; i < reversedInterceptors.Length; i++)
                {
                    var interceptor = reversedInterceptors[i];

                    if (interceptor.GetType().GetMethod(nameof(ICachingProcessInterceptor.OnCacheStoringAsync)) is MethodInfo onCacheStoringMethodInfo
                        && onCacheStoringMethodInfo.GetCustomAttribute<SkipCallAttribute>(false) is null)
                    {
                        var nextOnCacheStoring = _onCacheStoringWrappedDelegate;
                        _onCacheStoringWrappedDelegate = (actionContext, key, entry, next) =>
                        {
                            return interceptor.OnCacheStoringAsync(actionContext,
                                                                   key,
                                                                   entry,
                                                                   (actionContext, key, entry) => nextOnCacheStoring(actionContext, key, entry, next));
                        };
                    }

                    if (interceptor.GetType().GetMethod(nameof(ICachingProcessInterceptor.OnResponseWritingAsync)) is MethodInfo onResponseWritingMethodInfo
                        && onResponseWritingMethodInfo.GetCustomAttribute<SkipCallAttribute>(false) is null)
                    {
                        var nextOnResponseWriting = _onResponseWritingWrappedDelegate;
                        _onResponseWritingWrappedDelegate = (actionContext, entry, next) =>
                        {
                            return interceptor.OnResponseWritingAsync(actionContext,
                                                                      entry,
                                                                      (actionContext, entry) => nextOnResponseWriting(actionContext, entry, next));
                        };
                    }

                    if (interceptor.GetType().GetMethod(nameof(ICachingProcessInterceptor.OnResultSettingAsync)) is MethodInfo onResultSettingMethodInfo
                        && onResultSettingMethodInfo.GetCustomAttribute<SkipCallAttribute>(false) is null)
                    {
                        var nextOnResultSetting = _onResultSettingWrappedDelegate;
                        _onResultSettingWrappedDelegate = (actionContext, actionResult, next) =>
                        {
                            return interceptor.OnResultSettingAsync(actionContext,
                                                                    actionResult,
                                                                    (actionContext, actionResult) => nextOnResultSetting(actionContext, actionResult, next));
                        };
                    }
                }
            }
            _interceptors = interceptors;
        }

        #endregion Public 构造函数

        #region ICachingProcessInterceptor

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<ResponseCacheEntry> OnCacheStoringAsync(ActionContext actionContext,
                                                            string key,
                                                            ResponseCacheEntry entry,
                                                            Func<ActionContext, string, ResponseCacheEntry, Task<ResponseCacheEntry>> storeFunc)
        {
            return _onCacheStoringWrappedDelegate(actionContext, key, entry, new(storeFunc));
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> OnResponseWritingAsync(ActionContext actionContext,
                                                 ResponseCacheEntry entry,
                                                 Func<ActionContext, ResponseCacheEntry, Task<bool>> writeFunc)
        {
            return _onResponseWritingWrappedDelegate(actionContext, entry, new(writeFunc));
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task OnResultSettingAsync(ActionExecutingContext actionContext,
                                         IActionResult actionResult,
                                         Func<ActionExecutingContext, IActionResult, Task> setResultFunc)
        {
            return _onResultSettingWrappedDelegate(actionContext, actionResult, new(setResultFunc));
        }

        #endregion ICachingProcessInterceptor
    }
}