using System;
using System.Collections.Generic;

using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseCaching.Interceptors;

/// <summary>
/// 默认拦截器配置
/// </summary>
public class InterceptorOptions : IOptions<InterceptorOptions>
{
    #region Public 属性

    /// <summary>
    /// 拦截器实例集合
    /// </summary>
    public List<IResponseCachingInterceptor> CachingProcessInterceptors { get; } = new();

    /// <summary>
    /// 需要从 <see cref="IServiceProvider"/> 中获取的拦截器类型
    /// </summary>
    public List<Type> CachingProcessInterceptorTypes { get; } = new();

    /// <inheritdoc/>
    public InterceptorOptions Value => this;

    #endregion Public 属性
}