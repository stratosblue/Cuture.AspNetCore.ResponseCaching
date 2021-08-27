using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Generators
{
    /// <summary>
    /// 缓存Key生成器
    /// </summary>
    public interface ICacheKeyGenerator
    {
        #region Public 方法

        /// <summary>
        /// 生成缓存Key
        /// </summary>
        /// <param name="filterContext">当类型为 <see cref="FilterType.Action"/> 时，类型为 <see cref="ActionExecutingContext"/><para/>
        /// 当类型为 <see cref="FilterType.Resource"/> 时，类型为 <see cref="ResourceExecutingContext"/><para/>
        /// </param>
        /// <returns></returns>
        ValueTask<string> GenerateKeyAsync(FilterContext filterContext);

        #endregion Public 方法
    }
}