using System;

using Microsoft.Extensions.DependencyInjection;

namespace Cuture.AspNetCore.ResponseCaching;

/// <summary>
/// ResponseCaching服务构建类
/// </summary>
public class ResponseCachingServiceBuilder
{
    #region Public 属性

    /// <inheritdoc cref="IServiceCollection"/>
    public IServiceCollection Services { get; }

    #endregion Public 属性

    #region Public 构造函数

    /// <inheritdoc cref="ResponseCachingServiceBuilder"/>
    public ResponseCachingServiceBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    #endregion Public 构造函数
}