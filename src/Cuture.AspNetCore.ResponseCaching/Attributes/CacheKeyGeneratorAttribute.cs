using System;

using Cuture.AspNetCore.ResponseCaching;
using Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 指定缓存Key生成器
    /// <para/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CacheKeyGeneratorAttribute : Attribute
    {
        #region Public 属性

        /// <summary>
        /// 过滤器类型
        /// </summary>
        public FilterType FilterType { get; }

        /// <summary>
        /// 缓存Key生成器类型
        /// </summary>
        public Type Type { get; }

        #endregion Public 属性

        #region Public 构造函数

        /// <summary>
        /// <inheritdoc cref="CacheKeyGeneratorAttribute"/>
        /// </summary>
        /// <param name="type">
        /// 需要实现 <see cref="ICacheKeyGenerator"/> 接口
        /// <para/>
        /// 需要Attribute数据时实现 <see cref="IResponseCachingAttributeSetter"/> 接口
        /// </param>
        /// <param name="filterType"></param>
        public CacheKeyGeneratorAttribute(Type type, FilterType filterType)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(ICacheKeyGenerator).IsAssignableFrom(type))
            {
                throw new ArgumentException($"CacheKeyGenerator - {type} must derives from {nameof(ICacheKeyGenerator)}");
            }

            Type = type;
            FilterType = filterType;
        }

        #endregion Public 构造函数
    }
}