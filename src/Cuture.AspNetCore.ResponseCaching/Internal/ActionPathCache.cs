using System;
using System.Collections.Immutable;
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

    private ImmutableDictionary<string, char[]> _actionPathCache = ImmutableDictionary.Create<string, char[]>(StringComparer.Ordinal);

    #endregion Private 字段

    #region Public 方法

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> GetPath(ActionContext actionContext)
    {
        var id = actionContext.ActionDescriptor.Id;
        return _actionPathCache.TryGetValue(id, out var pathValue)
               ? pathValue
               : CreateLowerPath(actionContext, id);
    }

    #endregion Public 方法

    #region Private 方法

    private ReadOnlySpan<char> CreateLowerPath(ActionContext actionContext, string id)
    {
        var path = actionContext.HttpContext.Request.Path.Value.AsSpan().TrimEnd('/');
        var pathValue = new char[path.Length];
        path.ToLowerInvariant(pathValue);
        _actionPathCache = _actionPathCache.SetItem(id, pathValue);
        return pathValue;
    }

    #endregion Private 方法
}
