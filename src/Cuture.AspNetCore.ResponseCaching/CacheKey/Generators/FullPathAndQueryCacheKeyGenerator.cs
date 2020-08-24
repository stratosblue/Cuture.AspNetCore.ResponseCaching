using System.Threading.Tasks;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Generators
{
    /// <summary>
    /// 完整请求路径和查询字符串缓存键生成器
    /// </summary>
    public class FullPathAndQueryCacheKeyGenerator : ICacheKeyGenerator
    {
        /// <inheritdoc/>
        public ValueTask<string> GenerateKeyAsync(FilterContext filterContext) => new ValueTask<string>(filterContext.HttpContext.Request.GetEncodedPathAndQuery());
    }
}