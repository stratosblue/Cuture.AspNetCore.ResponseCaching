using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseCaching.Interceptors;

/// <summary>
/// <inheritdoc cref="IResponseCachingInterceptor"/> - 缓存写入响应
/// </summary>
public interface IResponseWritingInterceptor : IResponseCachingInterceptor
{
    #region Public 方法

    /// <summary>
    /// 缓存写入响应时拦截
    /// </summary>
    /// <param name="actionContext">Http请求方法的上下文<see cref="ActionContext"/></param>
    /// <param name="entry">缓存项</param>
    /// <param name="next">后续进行将缓存写入响应的方法委托</param>
    /// <returns>是否已写入响应</returns>
    Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next);

    #endregion Public 方法
}