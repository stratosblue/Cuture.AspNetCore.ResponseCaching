using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Builders
{
    /// <summary>
    /// 表单参数缓存键构建器
    /// </summary>
    public class FormKeysCacheKeyBuilder : CacheKeyBuilder
    {
        /// <summary>
        /// 表单键列表
        /// </summary>
        private readonly string[] _formKeys;

        /// <summary>
        /// 表单参数缓存键构建器
        /// </summary>
        /// <param name="innerBuilder"></param>
        /// <param name="strictMode"></param>
        /// <param name="formKeys"></param>
        public FormKeysCacheKeyBuilder(CacheKeyBuilder innerBuilder, CacheKeyStrictMode strictMode, IEnumerable<string> formKeys) : base(innerBuilder, strictMode)
        {
            _formKeys = formKeys?.ToArray() ?? throw new ArgumentNullException(nameof(formKeys));
        }

        /// <inheritdoc/>
        public override ValueTask<string> BuildAsync(FilterContext filterContext, StringBuilder keyBuilder)
        {
            if (!filterContext.HttpContext.Request.HasFormContentType)
            {
                if (!ProcessFormNotFind())
                {
                    return new ValueTask<string>();
                }
            }
            var form = filterContext.HttpContext.Request.Form;
            foreach (var key in _formKeys)
            {
                if (form.TryGetValue(key, out var value))
                {
                    keyBuilder.Append(CombineChar);
                    keyBuilder.Append(value);
                }
                else
                {
                    if (!ProcessKeyNotFind(key))
                    {
                        return new ValueTask<string>();
                    }
                }
            }
            return base.BuildAsync(filterContext, keyBuilder);
        }

        /// <summary>
        /// 处理未找到表单
        /// </summary>
        /// <returns></returns>
        protected bool ProcessFormNotFind()
        {
            return StrictMode switch
            {
                CacheKeyStrictMode.Ignore => true,
                CacheKeyStrictMode.Strict => false,
#pragma warning disable CA2208 // 正确实例化参数异常
                CacheKeyStrictMode.StrictWithException => throw new RequestFormNotFindException(),
#pragma warning restore CA2208 // 正确实例化参数异常
                _ => throw new ArgumentException($"Unhandleable CacheKeyStrictMode: {StrictMode}"),
            };
        }
    }
}