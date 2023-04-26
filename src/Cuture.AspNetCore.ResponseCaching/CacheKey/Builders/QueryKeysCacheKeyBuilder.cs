using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cuture.AspNetCore.ResponseCaching.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Builders;

/// <summary>
/// 请求查询参数缓存键构建器
/// </summary>
public class QueryKeysCacheKeyBuilder : CacheKeyBuilder
{
    #region Private 字段

    /// <summary>
    /// 请求查询参数列表
    /// </summary>
    private readonly string[] _queryKeys;

    #endregion Private 字段

    #region Public 构造函数

    /// <summary>
    /// 请求查询参数缓存键构建器
    /// </summary>
    /// <param name="innerBuilder"></param>
    /// <param name="strictMode"></param>
    /// <param name="queryKeys"></param>
    public QueryKeysCacheKeyBuilder(CacheKeyBuilder? innerBuilder, CacheKeyStrictMode strictMode, IEnumerable<string> queryKeys) : base(innerBuilder, strictMode)
    {
        _queryKeys = queryKeys?.ToArray() ?? throw new ArgumentNullException(nameof(queryKeys));
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public override ValueTask<string> BuildAsync(FilterContext filterContext, StringBuilder keyBuilder)
    {
        //未指定key，则使用全部
        if (_queryKeys.Length == 0)
        {
            var queryString = filterContext.HttpContext.Request.QueryString;
            if (queryString.HasValue)
            {
                char[]? buffer = null;
                try
                {
                    buffer = ArrayPool<char>.Shared.Rent(queryString.Value!.Length);
                    var length = QueryStringOrderUtil.Order(queryString, buffer);
                    keyBuilder.Append(CombineChar);
                    keyBuilder.Append(buffer, 0, length);
                }
                finally
                {
                    if (buffer is not null)
                    {
                        ArrayPool<char>.Shared.Return(buffer);
                    }
                }
            }

            return base.BuildAsync(filterContext, keyBuilder);
        }

        var query = filterContext.HttpContext.Request.Query;
        foreach (var queryKey in _queryKeys)
        {
            if (query.TryGetValue(queryKey, out var value))
            {
                keyBuilder.Append(CombineChar);
                keyBuilder.Append(queryKey);
                keyBuilder.Append('=');
                keyBuilder.Append(value);
            }
            else
            {
                if (!ProcessKeyNotFind(queryKey))
                {
                    return new ValueTask<string>();
                }
            }
        }
        return base.BuildAsync(filterContext, keyBuilder);
    }

    #endregion Public 方法
}
