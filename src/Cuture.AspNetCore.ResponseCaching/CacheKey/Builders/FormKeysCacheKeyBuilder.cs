using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Builders;

/// <summary>
/// 表单参数缓存键构建器
/// </summary>
public class FormKeysCacheKeyBuilder : CacheKeyBuilder
{
    #region Private 字段

    /// <summary>
    /// 表单键列表
    /// </summary>
    private readonly string[] _formKeys;

    #endregion Private 字段

    #region Public 构造函数

    /// <summary>
    /// 表单参数缓存键构建器
    /// </summary>
    /// <param name="innerBuilder"></param>
    /// <param name="strictMode"></param>
    /// <param name="formKeys"></param>
    public FormKeysCacheKeyBuilder(CacheKeyBuilder? innerBuilder, CacheKeyStrictMode strictMode, IEnumerable<string> formKeys) : base(innerBuilder, strictMode)
    {
        _formKeys = formKeys?.ToLowerArray() ?? throw new ArgumentNullException(nameof(formKeys));
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public override ValueTask<string> BuildAsync(FilterContext filterContext, StringBuilder keyBuilder)
    {
        if (!filterContext.HttpContext.Request.HasFormContentType)
        {
            if (!ProcessFormNotFound())
            {
                return default;
            }
        }

        keyBuilder.Append(CombineChar);

        var form = filterContext.HttpContext.Request.Form;

        foreach (var key in _formKeys)
        {
            if (form.TryGetValue(key, out var value))
            {
                keyBuilder.Append($"{key}={value}&");
            }
            else
            {
                if (!ProcessKeyNotFound(key))
                {
                    return default;
                }
            }
        }
        return base.BuildAsync(filterContext, keyBuilder.TrimEndAnd());
    }

    #endregion Public 方法

    #region Protected 方法

    /// <summary>
    /// 处理未找到表单
    /// </summary>
    /// <returns></returns>
    protected bool ProcessFormNotFound()
    {
        return StrictMode switch
        {
            CacheKeyStrictMode.Ignore => true,
            CacheKeyStrictMode.Strict => false,
            CacheKeyStrictMode.StrictWithException => throw new RequestFormNotFoundException(),
            _ => throw new ArgumentException($"Unhandleable CacheKeyStrictMode: {StrictMode}"),
        };
    }

    #endregion Protected 方法
}
