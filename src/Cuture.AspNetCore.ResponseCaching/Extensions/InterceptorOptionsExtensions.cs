using System;

using Cuture.AspNetCore.ResponseCaching.Interceptors;

namespace Cuture.AspNetCore.ResponseCaching;

/// <summary>
/// <see cref="InterceptorOptions"/> 拓展
/// </summary>
public static class InterceptorOptionsExtensions
{
    #region Public 方法

    /// <summary>
    /// 添加拦截器
    /// </summary>
    /// <typeparam name="TInterceptor"></typeparam>
    /// <param name="options"></param>
    /// <param name="interceptor"></param>
    /// <returns></returns>
    public static InterceptorOptions AddInterceptor<TInterceptor>(this InterceptorOptions options, TInterceptor interceptor) where TInterceptor : IResponseCachingInterceptor
    {
        options.CachingProcessInterceptors.Add(interceptor);
        return options;
    }

    /// <summary>
    /// 添加从 <see cref="IServiceProvider"/> 中获取的拦截器
    /// </summary>
    /// <typeparam name="TInterceptor"></typeparam>
    /// <param name="options"></param>
    /// <returns></returns>
    public static InterceptorOptions AddServiceInterceptor<TInterceptor>(this InterceptorOptions options) where TInterceptor : IResponseCachingInterceptor
    {
        options.CachingProcessInterceptorTypes.Add(typeof(TInterceptor));
        return options;
    }

    #endregion Public 方法
}