using System;
using System.Threading;

using Cuture.AspNetCore.ResponseCaching;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 表示标记的Action响应内容是可缓存的<para/>
/// 从 DI 容器中获取 <see cref="IResponseCachingFilterBuilder"/> 并构造对应的响应缓存 <see cref="IFilterMetadata"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ResponseCacheableAttribute : Attribute, IFilterFactory, IOrderedFilter
{
    #region Private 字段

    private SpinLock _createInstanceLock = new(false);

    private IFilterMetadata? _filterMetadata;

    #endregion Private 字段

    #region Public 属性

    /// <inheritdoc/>
    public bool IsReusable => true;

    /// <inheritdoc/>
    public int Order { get; set; }

    #endregion Public 属性

    #region Public 方法

    /// <inheritdoc/>
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var locked = false;
        try
        {
            _createInstanceLock.Enter(ref locked);
            if (_filterMetadata is null)
            {
                _filterMetadata = CreateFilter(serviceProvider);
            }
            return _filterMetadata;
        }
        finally
        {
            if (locked)
            {
                _createInstanceLock.Exit(false);
            }
        }
    }

    #endregion Public 方法

    #region Protected 方法

    /// <summary>
    /// 创建Filter（线程安全）
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    protected virtual IFilterMetadata CreateFilter(IServiceProvider serviceProvider)
    {
        var filterBuilder = serviceProvider.GetRequiredService<IResponseCachingFilterBuilder>();

        return filterBuilder.CreateFilter(serviceProvider);
    }

    #endregion Protected 方法
}
