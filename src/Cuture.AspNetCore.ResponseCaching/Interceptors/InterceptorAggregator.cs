using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Interceptors
{
    /// <summary>
    /// 委托 - <inheritdoc cref="ICacheStoringInterceptor.OnCacheStoringAsync(ActionContext, string, ResponseCacheEntry, OnCacheStoringDelegate)"/>
    /// </summary>
    /// <param name="actionContext"></param>
    /// <param name="key"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public delegate Task<ResponseCacheEntry?> OnCacheStoringDelegate(ActionContext actionContext, string key, ResponseCacheEntry entry);

    /// <summary>
    /// 委托 - <inheritdoc cref="IResponseWritingInterceptor.OnResponseWritingAsync(ActionContext, ResponseCacheEntry, OnResponseWritingDelegate)"/>
    /// </summary>
    /// <param name="actionContext"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public delegate Task<bool> OnResponseWritingDelegate(ActionContext actionContext, ResponseCacheEntry entry);

    /// <summary>
    /// 委托 - <inheritdoc cref="IActionResultSettingInterceptor.OnResultSettingAsync(ActionExecutingContext, IActionResult, OnResultSettingDelegate)"/>
    /// </summary>
    /// <param name="actionContext"></param>
    /// <param name="actionResult"></param>
    /// <returns></returns>
    public delegate Task OnResultSettingDelegate(ActionExecutingContext actionContext, IActionResult actionResult);

    /// <summary>
    /// 拦截器集合
    /// </summary>
    public class InterceptorAggregator : IActionResultSettingInterceptor, IResponseWritingInterceptor, ICacheStoringInterceptor
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
        public delegate Task<ResponseCacheEntry?> OnCacheStoringWrappedDelegate(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate next);

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

        private readonly IEnumerable<IResponseCachingInterceptor>? _interceptors;

        private readonly OnCacheStoringWrappedDelegate _onCacheStoringWrappedDelegate;
        private readonly OnResponseWritingWrappedDelegate _onResponseWritingWrappedDelegate;
        private readonly OnResultSettingWrappedDelegate _onResultSettingWrappedDelegate;

        #endregion Private 字段

        #region Public 构造函数

        /// <summary>
        /// 拦截器集合
        /// </summary>
        /// <param name="interceptors"><see cref="IResponseCachingInterceptor"/></param>
        public InterceptorAggregator(IEnumerable<IResponseCachingInterceptor>? interceptors)
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

                    if (interceptor is ICacheStoringInterceptor cacheStoringInterceptor)
                    {
                        var nextOnCacheStoring = _onCacheStoringWrappedDelegate;
                        _onCacheStoringWrappedDelegate = (actionContext, key, entry, next) =>
                        {
                            return cacheStoringInterceptor.OnCacheStoringAsync(actionContext,
                                                                               key,
                                                                               entry,
                                                                               (actionContext, key, entry) => nextOnCacheStoring(actionContext, key, entry, next));
                        };
                    }

                    if (interceptor is IResponseWritingInterceptor responseWritingInterceptor)
                    {
                        var nextOnResponseWriting = _onResponseWritingWrappedDelegate;
                        _onResponseWritingWrappedDelegate = (actionContext, entry, next) =>
                        {
                            return responseWritingInterceptor.OnResponseWritingAsync(actionContext,
                                                                                     entry,
                                                                                     (actionContext, entry) => nextOnResponseWriting(actionContext, entry, next));
                        };
                    }

                    if (interceptor is IActionResultSettingInterceptor actionResultSettingInterceptor)
                    {
                        var nextOnResultSetting = _onResultSettingWrappedDelegate;
                        _onResultSettingWrappedDelegate = (actionContext, actionResult, next) =>
                        {
                            return actionResultSettingInterceptor.OnResultSettingAsync(actionContext,
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
        public Task<ResponseCacheEntry?> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate next)
            => _onCacheStoringWrappedDelegate(actionContext, key, entry, next);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next)
            => _onResponseWritingWrappedDelegate(actionContext, entry, next);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task OnResultSettingAsync(ActionExecutingContext actionContext, IActionResult actionResult, OnResultSettingDelegate next)
            => _onResultSettingWrappedDelegate(actionContext, actionResult, next);

        #endregion ICachingProcessInterceptor
    }
}