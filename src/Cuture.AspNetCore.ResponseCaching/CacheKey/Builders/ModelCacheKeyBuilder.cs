﻿using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Builders;

/// <summary>
/// 参数Model缓存键构建器
/// </summary>
public class ModelCacheKeyBuilder : CacheKeyBuilder
{
    #region Private 字段

    private readonly IModelKeyParser _modelKeyParser;

    /// <summary>
    /// model名列表
    /// </summary>
    private readonly string[] _modelNames;

    /// <summary>
    /// 使用所有Model
    /// </summary>
    private readonly bool _useAllModel;

    #endregion Private 字段

    #region Public 构造函数

    /// <summary>
    /// 参数Model缓存键构建器
    /// </summary>
    /// <param name="innerBuilder"></param>
    /// <param name="strictMode"></param>
    /// <param name="modelNames"></param>
    /// <param name="modelKeyParser"></param>
    public ModelCacheKeyBuilder(CacheKeyBuilder? innerBuilder,
                                CacheKeyStrictMode strictMode,
                                IEnumerable<string> modelNames,
                                IModelKeyParser modelKeyParser) : base(innerBuilder, strictMode)
    {
        _modelNames = modelNames?.ToLowerArray() ?? throw new ArgumentNullException(nameof(modelNames));
        _useAllModel = _modelNames.Length == 0;
        _modelKeyParser = modelKeyParser;
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public override ValueTask<string> BuildAsync(FilterContext filterContext, StringBuilder keyBuilder)
    {
        if (filterContext is ActionExecutingContext executingContext)
        {
            keyBuilder.Append(CombineChar);

            if (_useAllModel)
            {
                foreach (var item in executingContext.ActionArguments.OrderBy(static m => m.Key))
                {
                    keyBuilder.Append($"{item.Key}={_modelKeyParser.Parse(item.Value)}&");
                }
                return base.BuildAsync(filterContext, keyBuilder.TrimEndAnd());
            }

            foreach (var key in _modelNames)
            {
                if (executingContext.ActionArguments.TryGetValue(key, out var value))
                {
                    keyBuilder.Append($"{key}={_modelKeyParser.Parse(value)}&");
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

        throw new ResponseCachingException("Not found ActionArguments in HttpContext.Items");
    }

    #endregion Public 方法
}
