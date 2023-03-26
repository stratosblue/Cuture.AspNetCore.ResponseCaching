using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching;

/// <summary>
/// 默认响应缓存确定器
/// </summary>
internal class DefaultResponseCacheDeterminer : IResponseCacheDeterminer
{
    #region Public 方法

    /// <inheritdoc/>
    public bool CanCaching(ResourceExecutedContext context, ResponseCacheEntry cacheEntry)
        => context.HttpContext.Response.StatusCode == StatusCodes.Status200OK;

    /// <inheritdoc/>
    public bool CanCaching(ActionExecutedContext context) => context.Result != null;

    #endregion Public 方法
}
