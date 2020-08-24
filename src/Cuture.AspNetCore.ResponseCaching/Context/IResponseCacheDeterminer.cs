using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 响应缓存确定器
    /// </summary>
    public interface IResponseCacheDeterminer
    {
        /// <summary>
        /// 是否可以缓存此次请求
        /// <para/>
        /// 默认仅在使用 <see cref="FilterType.Resource"/> 类型的过滤器 (<see cref="IAsyncResourceFilter"/>) 时生效
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cacheEntry"></param>
        /// <returns></returns>
        bool CanCaching(ResourceExecutedContext context, ResponseCacheEntry cacheEntry);

        /// <summary>
        /// 是否可以缓存此次请求
        /// <para/>
        /// 仅在使用 <see cref="FilterType.Action"/> 类型的过滤器 (<see cref="IAsyncActionFilter"/>) 时生效
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool CanCaching(ActionExecutedContext context);
    }
}