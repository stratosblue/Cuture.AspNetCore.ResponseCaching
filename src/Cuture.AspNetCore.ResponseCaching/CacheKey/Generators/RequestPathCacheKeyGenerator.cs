using System.Buffers;
using Cuture.AspNetCore.ResponseCaching.Internal;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;

/// <summary>
/// 请求路径缓存键生成器
/// </summary>
public class RequestPathCacheKeyGenerator : ICacheKeyGenerator
{
    #region Private 字段

    private readonly ActionPathCache _actionPathCache = new();

    #endregion Private 字段

    #region Public 方法

    /// <inheritdoc/>
    public ValueTask<string> GenerateKeyAsync(FilterContext filterContext)
    {
        var method = filterContext.HttpContext.Request.NormalizeMethodNameAsKeyPrefix();

        char[]? buffer = null;
        try
        {
            using var pathValue = _actionPathCache.GetPath(filterContext);
            var path = pathValue.Value;
            var length = method.Length + path.Length;
            buffer = ArrayPool<char>.Shared.Rent(length);
            method.CopyTo(buffer);
            path.CopyTo(buffer.AsSpan(method.Length));
            return new(new string(buffer, 0, length));
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
