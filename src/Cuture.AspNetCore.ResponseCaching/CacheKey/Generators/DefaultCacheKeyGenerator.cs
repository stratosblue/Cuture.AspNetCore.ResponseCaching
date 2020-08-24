using System;
using System.Text;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.CacheKey.Builders;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.ObjectPool;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Generators
{
    /// <summary>
    /// 缓存键生成器
    /// </summary>
    public class DefaultCacheKeyGenerator : ICacheKeyGenerator
    {
        private readonly CacheKeyBuilder _innerBuilder;

        private readonly ObjectPool<StringBuilder> _stringBuilderPool;

        /// <summary>
        /// 缓存键生成器
        /// </summary>
        /// <param name="innerBuilder"></param>
        public DefaultCacheKeyGenerator(CacheKeyBuilder innerBuilder)
        {
            _innerBuilder = innerBuilder ?? throw new ArgumentNullException(nameof(innerBuilder));
            _stringBuilderPool = new DefaultObjectPoolProvider().CreateStringBuilderPool();
        }

        /// <inheritdoc/>
        public async ValueTask<string> GenerateKeyAsync(FilterContext filterContext)
        {
            var keyBuilder = _stringBuilderPool.Get();
            try
            {
                var path = filterContext.HttpContext.Request.Path.Value;

                if (path.EndsWith('/'))
                {
                    keyBuilder.Append(path, 0, path.Length - 1);
                }
                else
                {
                    keyBuilder.Append(path, 0, path.Length);
                }
                return await _innerBuilder.BuildAsync(filterContext, keyBuilder);
            }
            finally
            {
                _stringBuilderPool.Return(keyBuilder);
            }
        }
    }
}