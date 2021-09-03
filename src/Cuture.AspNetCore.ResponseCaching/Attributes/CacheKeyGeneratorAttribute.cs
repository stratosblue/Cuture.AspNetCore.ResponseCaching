using System;

using Cuture.AspNetCore.ResponseCaching;
using Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;
using Cuture.AspNetCore.ResponseCaching.Metadatas;

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 指定缓存Key生成器
    /// <para/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CacheKeyGeneratorAttribute : Attribute, ICacheKeyGeneratorMetadata
    {
        #region Public 属性

        /// <inheritdoc/>
        public Type CacheKeyGeneratorType { get; }

        /// <inheritdoc/>
        public FilterType FilterType { get; }

        #endregion Public 属性

        #region Public 构造函数

        /// <summary>
        /// <inheritdoc cref="CacheKeyGeneratorAttribute"/>
        /// </summary>
        /// <param name="type">
        /// 需要实现 <see cref="ICacheKeyGenerator"/> 接口
        /// <para/>
        /// 需要 Action 的 <see cref="Endpoint"/> 时, 实现 <see cref="IEndpointSetter"/> 接口
        /// </param>
        /// <param name="filterType"></param>
        public CacheKeyGeneratorAttribute(Type type, FilterType filterType)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            CacheKeyGeneratorType = Checks.ThrowIfNotICacheKeyGenerator(type);
            FilterType = filterType;
        }

        #endregion Public 构造函数
    }
}