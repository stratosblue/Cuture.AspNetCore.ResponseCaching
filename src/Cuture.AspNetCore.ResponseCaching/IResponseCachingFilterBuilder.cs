using System;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching;

/// <summary>
/// 响应缓存 <see cref="IFilterMetadata"/> 构建器
/// </summary>
public interface IResponseCachingFilterBuilder
{
    #region Public 方法

    /// <summary>
    /// 创建 <see cref="IFilterMetadata"/>
    /// </summary>
    /// <param name="serviceProvider">作用域为 触发构建Filter的请求 的 <see cref="IServiceProvider"/></param>
    /// <returns></returns>
    IFilterMetadata CreateFilter(IServiceProvider serviceProvider);

    #endregion Public 方法
}