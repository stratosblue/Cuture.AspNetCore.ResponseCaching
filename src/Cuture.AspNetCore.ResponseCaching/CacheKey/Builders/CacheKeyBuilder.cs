using System;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseCaching.CacheKey.Builders
{
    /// <summary>
    /// 缓存键构建器
    /// </summary>
    public abstract class CacheKeyBuilder
    {
        /// <summary>
        /// 连接Key字符
        /// </summary>
        public const char CombineChar = ResponseCachingConstants.CombineChar;

        private readonly CacheKeyBuilder _innerBuilder;

        /// <summary>
        /// 严格模式
        /// </summary>
        public CacheKeyStrictMode StrictMode { get; }

        /// <summary>
        /// 缓存键构建器
        /// </summary>
        /// <param name="innerBuilder"></param>
        /// <param name="strictMode"></param>
        public CacheKeyBuilder(CacheKeyBuilder innerBuilder, CacheKeyStrictMode strictMode)
        {
            _innerBuilder = innerBuilder;
            StrictMode = strictMode;
        }

        /// <summary>
        /// 构建Key
        /// </summary>
        /// <param name="filterContext">Filter上下文</param>
        /// <param name="keyBuilder"></param>
        /// <returns></returns>
        public virtual ValueTask<string> BuildAsync(FilterContext filterContext, StringBuilder keyBuilder) => _innerBuilder is null ? new ValueTask<string>(keyBuilder.ToString()) : _innerBuilder.BuildAsync(filterContext, keyBuilder);

        /// <summary>
        /// 处理未找到key的情况
        /// </summary>
        /// <param name="notFindKeyName"></param>
        /// <returns></returns>
        protected bool ProcessKeyNotFind(string notFindKeyName)
        {
            switch (StrictMode)
            {
                case CacheKeyStrictMode.Ignore:
                    return true;

                case CacheKeyStrictMode.Strict:
                    return false;

                case CacheKeyStrictMode.StrictWithException:
                    throw new CacheVaryKeyNotFindException(notFindKeyName);

                default:
                    throw new ArgumentException($"Unhandleable CacheKeyStrictMode: {StrictMode}");
            }
        }
    }
}