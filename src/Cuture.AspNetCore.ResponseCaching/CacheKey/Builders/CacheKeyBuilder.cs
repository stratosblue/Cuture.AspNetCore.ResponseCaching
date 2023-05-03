using System;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Builders;

/// <summary>
/// 缓存键构建器
/// </summary>
public abstract class CacheKeyBuilder
{
    #region const

    /// <summary>
    /// 连接Key字符
    /// </summary>
    public const char CombineChar = ResponseCachingConstants.CombineChar;

    #endregion const

    #region Private 字段

    private readonly CacheKeyBuilder? _innerBuilder;

    #endregion Private 字段

    #region Public 属性

    /// <summary>
    /// 严格模式
    /// </summary>
    public CacheKeyStrictMode StrictMode { get; }

    #endregion Public 属性

    #region Public 构造函数

    /// <summary>
    /// 缓存键构建器
    /// </summary>
    /// <param name="innerBuilder"></param>
    /// <param name="strictMode"></param>
    public CacheKeyBuilder(CacheKeyBuilder? innerBuilder, CacheKeyStrictMode strictMode)
    {
        _innerBuilder = innerBuilder;
        StrictMode = strictMode;
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <summary>
    /// 构建Key
    /// </summary>
    /// <param name="filterContext">Filter上下文</param>
    /// <param name="keyBuilder"></param>
    /// <returns></returns>
    public virtual ValueTask<string> BuildAsync(FilterContext filterContext, StringBuilder keyBuilder)
    {
        return _innerBuilder is null
               ? new(keyBuilder.ToString())
               : _innerBuilder.BuildAsync(filterContext, keyBuilder);
    }

    #endregion Public 方法

    #region Protected 方法

    /// <summary>
    /// 处理未找到key的情况
    /// </summary>
    /// <param name="notFoundKeyName"></param>
    /// <returns></returns>
    protected bool ProcessKeyNotFound(string notFoundKeyName)
    {
        return StrictMode switch
        {
            CacheKeyStrictMode.Ignore => true,
            CacheKeyStrictMode.Strict => false,
            CacheKeyStrictMode.StrictWithException => throw new CacheVaryKeyNotFoundException(notFoundKeyName),
            _ => throw new ArgumentException($"Unhandleable CacheKeyStrictMode: {StrictMode}"),
        };
    }

    #endregion Protected 方法
}
