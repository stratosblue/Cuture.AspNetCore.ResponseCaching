using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Builders;

/// <summary>
/// Claims缓存键构建器
/// </summary>
public class ClaimsCacheKeyBuilder : CacheKeyBuilder
{
    #region Private 字段

    /// <summary>
    /// ClaimType列表
    /// </summary>
    private readonly string[] _claimTypes;

    #endregion Private 字段

    #region Public 构造函数

    /// <summary>
    /// Claims缓存键构建器
    /// </summary>
    /// <param name="innerBuilder"></param>
    /// <param name="strictMode"></param>
    /// <param name="claimTypes">ClaimType列表</param>
    public ClaimsCacheKeyBuilder(CacheKeyBuilder? innerBuilder, CacheKeyStrictMode strictMode, IEnumerable<string> claimTypes) : base(innerBuilder, strictMode)
    {
        _claimTypes = claimTypes?.ToArray() ?? throw new ArgumentNullException(nameof(claimTypes));
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public override ValueTask<string> BuildAsync(FilterContext filterContext, StringBuilder keyBuilder)
    {
        foreach (var claimType in _claimTypes)
        {
            if (filterContext.HttpContext.User.FindFirst(claimType) is Claim claim)
            {
                keyBuilder.Append(CombineChar);
                keyBuilder.Append(claim.Value);
            }
            else
            {
                if (!ProcessKeyNotFind(claimType))
                {
                    return new ValueTask<string>();
                }
            }
        }
        return base.BuildAsync(filterContext, keyBuilder);
    }

    #endregion Public 方法
}