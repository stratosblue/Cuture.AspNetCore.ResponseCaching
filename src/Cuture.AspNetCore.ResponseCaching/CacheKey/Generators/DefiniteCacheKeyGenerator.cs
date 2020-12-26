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
        #region Private 字段

        private readonly string _key;

        #endregion Private 字段

        #region Public 构造函数

        /// <summary>
        /// 确定值缓存键生成器
        /// </summary>
        /// <param name="key"></param>
        public DefiniteCacheKeyGenerator(string key)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public ValueTask<string> GenerateKeyAsync(FilterContext filterContext) => new ValueTask<string>(_key);

        #endregion Public 方法
    }
}