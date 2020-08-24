using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Generators
{
    /// <summary>
    /// 缓存Key生成器
    /// </summary>
    public interface ICacheKeyGenerator
    {
        /// <summary>
        /// 生成缓存Key
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        ValueTask<string> GenerateKeyAsync(FilterContext filterContext);
    }
}