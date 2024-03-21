using System.Buffers;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Cuture.AspNetCore.ResponseCaching.Internal;

/// <summary>
/// <see cref="ActionDescriptor"/> 对应的 请求路径缓存
/// </summary>
internal sealed class ActionPathCache
{
    #region Private 字段

    private ImmutableDictionary<string, char[]?> _actionPathCache = ImmutableDictionary.Create<string, char[]?>(StringComparer.Ordinal);

    #endregion Private 字段

    #region Public 方法

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PooledReadOnlyCharSpan GetPath(ActionContext actionContext)
    {
        var id = actionContext.ActionDescriptor.Id;
        var result = _actionPathCache.TryGetValue(id, out var cachedValue)
                     ? cachedValue is null
                         ? GetRequestPath(actionContext)
                         : new PooledReadOnlyCharSpan(null, cachedValue)
                     : GetAndCacheRequestPath(actionContext, id);

        Debug.Assert(string.Equals(actionContext.HttpContext.Request.Path.ToString(), result.ToString(), StringComparison.OrdinalIgnoreCase));

        return result;
    }

    #endregion Public 方法

    #region Private 方法

    private static PooledReadOnlyCharSpan GetRequestPath(ActionContext actionContext)
    {
        var path = actionContext.HttpContext.Request.Path.Value.AsSpan().TrimEnd('/');
        var buffer = ArrayPool<char>.Shared.Rent(path.Length);
        return new(buffer, buffer.AsSpan(0, path.ToLowerInvariant(buffer)));
    }

    private PooledReadOnlyCharSpan GetAndCacheRequestPath(ActionContext actionContext, string id)
    {
        var pathValue = GetRequestPath(actionContext);
        var routeTemplate = actionContext.ActionDescriptor.AttributeRouteInfo?.Template;

        //路由模板或非ApiController缓存null，每次获取当次请求的path
        if (string.IsNullOrEmpty(routeTemplate)
            || routeTemplate.Contains('{'))
        {
            _actionPathCache = _actionPathCache.SetItem(id, null);
        }
        else
        {
            _actionPathCache = _actionPathCache.SetItem(id, pathValue.Value.ToArray());
        }
        return pathValue;
    }

    #endregion Private 方法
}
