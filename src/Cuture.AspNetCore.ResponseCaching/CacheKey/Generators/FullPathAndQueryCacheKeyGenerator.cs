using System;
using System.Buffers;
using System.Threading.Tasks;
using Cuture.AspNetCore.ResponseCaching.Internal;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;

/// <summary>
/// 完整请求路径和查询字符串缓存键生成器
/// </summary>
public class FullPathAndQueryCacheKeyGenerator : ICacheKeyGenerator
{
    #region Private 字段

    private readonly ActionPathCache _actionPathCache = new();

    #endregion Private 字段

    #region Public 方法

    /// <inheritdoc/>
    public ValueTask<string> GenerateKeyAsync(FilterContext filterContext)
    {
        var method = filterContext.HttpContext.Request.NormalizeMethodNameAsKeyPrefix();

        using var pathValue = _actionPathCache.GetPath(filterContext);

        var queryString = filterContext.HttpContext.Request.QueryString;
        if (!queryString.HasValue)
        {
            return new(new string(method) + pathValue.ToString());
        }

        char[]? buffer = null;
        try
        {
            var path = pathValue.Value;
            buffer = ArrayPool<char>.Shared.Rent(method.Length + path.Length + queryString.Value!.Length);

            var span = buffer.AsSpan();
            method.CopyTo(span);
            span = span.Slice(method.Length);

            path.CopyTo(span);
            span = span.Slice(path.Length);

            span[0] = ResponseCachingConstants.CombineChar;

            var length = QueryStringOrderUtil.Order(queryString, span.Slice(1));

            return new(new string(value: buffer, startIndex: 0, length: method.Length + path.Length + length));
        }
        finally
        {
            if (buffer is not null)
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
        }
    }

    #endregion Public 方法
}
