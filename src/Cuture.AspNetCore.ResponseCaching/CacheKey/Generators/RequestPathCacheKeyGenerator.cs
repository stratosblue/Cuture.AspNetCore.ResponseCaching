using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Generators
{
    /// <summary>
    /// 请求路径缓存键生成器
    /// </summary>
    public class RequestPathCacheKeyGenerator : ICacheKeyGenerator
    {
        /// <inheritdoc/>
        public ValueTask<string> GenerateKeyAsync(FilterContext filterContext)
        {
            var path = filterContext.HttpContext.Request.Path.Value;
            if (path.EndsWith('/'))
            {
                return new ValueTask<string>(path.Substring(0, path.Length - 1));
            }
            return new ValueTask<string>(path);
        }
    }
}