using System;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 本地热数据缓存
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class HotDataCacheAttribute : Attribute, IHotDataCacheBuilder
    {
        #region Public 属性

        /// <summary>
        /// 缓存策略
        /// </summary>
        public HotDataCachePolicy CachePolicy { get; set; } = HotDataCachePolicy.Default;

        /// <summary>
        /// 缓存热数据的数量
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }

        /// <inheritdoc cref="HotDataCacheAttribute(int, HotDataCachePolicy)"/>
        public HotDataCacheAttribute(int capacity)
        {
            Capacity = capacity;
        }

        /// <summary>
        /// <inheritdoc cref="HotDataCacheAttribute"/>
        /// </summary>
        /// <param name="capacity">缓存热数据的数量</param>
        /// <param name="cachePolicy">缓存替换策略</param>
        public HotDataCacheAttribute(int capacity, HotDataCachePolicy cachePolicy)
        {
            Capacity = capacity;
            CachePolicy = cachePolicy;
        }

        #endregion Public 属性

        #region Public 方法

        /// <inheritdoc/>
        public IHotDataCache Build(IServiceProvider serviceProvider)
        {
            var hotDataCacheProvider = serviceProvider.GetRequiredService<IHotDataCacheProvider>();
            return hotDataCacheProvider.Get(serviceProvider, Name ?? string.Empty, CachePolicy, Capacity);
        }

        #endregion Public 方法
    }
}