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
    /// 请求头缓存键构建器
    /// </summary>
    public class RequestHeadersCacheKeyBuilder : CacheKeyBuilder
    {
        #region Private 字段

        /// <summary>
        /// 请求头列表
        /// </summary>
        private readonly string[] _headers;

        #endregion Private 字段

        #region Public 构造函数

        /// <summary>
        /// 请求头缓存键构建器
        /// </summary>
        /// <param name="innerBuilder"></param>
        /// <param name="strictMode"></param>
        /// <param name="headers"></param>
        public RequestHeadersCacheKeyBuilder(CacheKeyBuilder innerBuilder, CacheKeyStrictMode strictMode, IEnumerable<string> headers) : base(innerBuilder, strictMode)
        {
            _headers = headers?.ToArray() ?? throw new ArgumentNullException(nameof(headers));
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public override ValueTask<string> BuildAsync(FilterContext filterContext, StringBuilder keyBuilder)
        {
            var headers = filterContext.HttpContext.Request.Headers;
            foreach (var header in _headers)
            {
                if (headers.TryGetValue(header, out var value))
                {
                    keyBuilder.Append(CombineChar);
                    keyBuilder.Append(value);
                }
                else
                {
                    if (!ProcessKeyNotFind(header))
                    {
                        return new ValueTask<string>();
                    }
                }
            }
            return base.BuildAsync(filterContext, keyBuilder);
        }

        #endregion Public 方法
    }
}