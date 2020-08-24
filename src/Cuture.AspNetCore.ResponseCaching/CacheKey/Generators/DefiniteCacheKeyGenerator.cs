using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Generators
{
    /// <summary>
    /// 确定值缓存键生成器
    /// </summary>
    public class DefiniteCacheKeyGenerator : ICacheKeyGenerator
    {
        private readonly string _key;

        /// <summary>
        /// 确定值缓存键生成器
        /// </summary>
        /// <param name="key"></param>
        public DefiniteCacheKeyGenerator(string key)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
        }

        /// <inheritdoc/>
        public ValueTask<string> GenerateKeyAsync(FilterContext filterContext) => new ValueTask<string>(_key);
    }
}