using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.Internal;

internal static class DefaultExecutionLockTimeoutFallback
{
    #region Public 方法

    public static Task SetStatus429(string cacheKey, FilterContext filterContext, Func<Task> next)
    {
        filterContext.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        return Task.CompletedTask;
    }

    #endregion Public 方法
}
