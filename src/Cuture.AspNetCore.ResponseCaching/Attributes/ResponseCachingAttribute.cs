using System;

using Cuture.AspNetCore.ResponseCaching;
using Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;
using Cuture.AspNetCore.ResponseCaching.Interceptors;
using Cuture.AspNetCore.ResponseCaching.Metadatas;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 响应缓存
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ResponseCachingAttribute
        : ResponseCacheableAttribute
        , IResponseDurationMetadata
        , IMaxCacheableResponseLengthMetadata
        , IResponseCacheModeMetadata
        , IResponseCacheKeyStrictModeMetadata
    {
        #region Public 属性

        /// <summary>
        /// Dump响应时的<see cref="System.IO.MemoryStream"/>初始化大小
        /// <para/>
        /// 不能小于 <see cref="ResponseCachingConstants.DefaultMinMaxCacheableResponseLength"/>
        /// </summary>
        [Obsolete("Use \"ResponseDumpCapacityAttribute\" instead.", true)]
        public int DumpCapacity { get; }

        /// <inheritdoc/>
        public int Duration { get; set; }

        /// <summary>
        /// 缓存通行模式（设置执行action的并发控制）
        /// <para/>
        /// Note:
        /// <para/>
        /// * 越细粒度的控制会带来相对更多的性能消耗
        /// <para/>
        /// * 虽然已经尽可能的实现了并发控制，仍然最好不要依赖此功能实现具体业务
        /// <para/>
        /// * 默认实现对 ActionFilter 的锁定效果不敢保证100%
        /// </summary>
        [Obsolete("使用 ExecutingLockAttribute 替代此属性", true)]
        public ExecutingLockMode LockMode { get; } = ExecutingLockMode.Default;

        /// <inheritdoc/>
        public int? MaxCacheableResponseLength { get; set; }

        /// <inheritdoc/>
        public CacheMode Mode { get; set; } = CacheMode.Default;

        /// <summary>
        /// 缓存数据存储位置
        /// </summary>
        public CacheStoreLocation StoreLocation { get; set; } = CacheStoreLocation.Default;

        /// <inheritdoc/>
        public CacheKeyStrictMode? StrictMode { get; set; }

        /// <summary>
        /// 依据声明
        /// </summary>
        public string[]? VaryByClaims { get; set; }

        /// <summary>
        /// 依据表单键
        /// </summary>
        public string[]? VaryByFormKeys { get; set; }

        /// <summary>
        /// 依据请求头
        /// </summary>
        public string[]? VaryByHeaders { get; set; }

        /// <summary>
        /// 依据Model
        /// <para/>
        /// Note:
        /// <para/>
        /// * 以下为使用默认实现时的情况
        /// <para/>
        /// * 使用空数组时为获取所有model进行生成Key
        /// <para/>
        /// * 使用的Filter将会从 <see cref="IAsyncResourceFilter"/> 转变为 <see cref="IAsyncActionFilter"/> &amp; <see cref="IAsyncResourceFilter"/>
        /// <para/>
        /// * 由于内部的实现问题，<see cref="LockMode"/> 的设置在某些情况下可能无法严格限制所有请求
        /// <para/>
        /// * 生成Key时，如果没有指定 <see cref="ModelKeyParserType"/>，
        /// 则检查Model是否实现 <see cref="ICacheKeyable"/> 接口，如果Model未实现 <see cref="ICacheKeyable"/> 接口，
        /// 则调用Model的 <see cref="object.ToString"/> 方法生成Key
        /// </summary>
        public string[]? VaryByModels { get; set; }

        /// <summary>
        /// 依据查询键
        /// </summary>
        public string[]? VaryByQueryKeys { get; set; }

        #region Types

        /// <summary>
        /// 缓存处理拦截器类型
        /// <para/>
        /// 需要实现 <see cref="ICachingProcessInterceptor"/> 接口
        /// </summary>
        [Obsolete("Use custom implementation \"CachingProcessInterceptor\" attribute instead.", true)]
        public Type? CachingProcessInterceptorType { get; }

        /// <summary>
        /// 自定义缓存键生成器类型
        /// <para/>
        /// 需要实现 <see cref="ICustomCacheKeyGenerator"/> 接口
        /// <para/>
        /// 需要Attribute数据时实现 <see cref="IResponseCachingAttributeSetter"/> 接口
        /// </summary>
        [Obsolete("Use \"CacheKeyGeneratorAttribute\" instead.", true)]
        public Type? CustomCacheKeyGeneratorType { get; }

        /// <summary>
        /// Model的Key解析器类型
        /// <para/>
        /// 需要实现 <see cref="IModelKeyParser"/> 接口
        /// </summary>
        [Obsolete("Use \"CacheModelKeyParserAttribute\" instead.", true)]
        public Type? ModelKeyParserType { get; }

        #endregion Types

        #endregion Public 属性

        #region Public 构造函数

        /// <summary>
        /// 响应缓存
        /// </summary>
        public ResponseCachingAttribute()
        {
        }

        /// <summary>
        /// 响应缓存
        /// </summary>
        /// <param name="duration">缓存时长（秒）</param>
        public ResponseCachingAttribute(int duration)
        {
            Duration = duration;
        }

        /// <summary>
        /// 响应缓存
        /// </summary>
        /// <param name="duration">缓存时长（秒）</param>
        /// <param name="mode"></param>
        public ResponseCachingAttribute(int duration, CacheMode mode) : this(duration)
        {
            Mode = mode;
        }

        /// <summary>
        /// 响应缓存
        /// </summary>
        /// <param name="duration">缓存时长（秒）</param>
        /// <param name="mode"></param>
        /// <param name="storeLocation"></param>
        public ResponseCachingAttribute(int duration, CacheMode mode, CacheStoreLocation storeLocation) : this(duration)
        {
            Mode = mode;
            StoreLocation = storeLocation;
        }

        /// <summary>
        /// 响应缓存
        /// </summary>
        /// <param name="duration">缓存时长（秒）</param>
        /// <param name="mode"></param>
        /// <param name="storeLocation"></param>
        /// <param name="strictMode"></param>
        public ResponseCachingAttribute(int duration, CacheMode mode, CacheStoreLocation storeLocation, CacheKeyStrictMode strictMode) : this(duration)
        {
            Mode = mode;
            StoreLocation = storeLocation;
            StrictMode = strictMode;
        }

        /// <summary>
        /// 响应缓存
        /// </summary>
        /// <param name="duration">缓存时长（秒）</param>
        /// <param name="mode"></param>
        /// <param name="storeLocation"></param>
        /// <param name="lockMode"></param>
        [Obsolete("使用 ExecutingLockAttribute 替代 ExecutingLockMode 设置", true)]
        public ResponseCachingAttribute(int duration, CacheMode mode, CacheStoreLocation storeLocation, ExecutingLockMode lockMode) : this(duration)
        {
            Mode = mode;
            StoreLocation = storeLocation;
            LockMode = lockMode;
        }

        /// <summary>
        /// 响应缓存
        /// </summary>
        /// <param name="duration">缓存时长（秒）</param>
        /// <param name="mode"></param>
        /// <param name="storeLocation"></param>
        /// <param name="strictMode"></param>
        /// <param name="lockMode"></param>
        [Obsolete("使用 ExecutingLockAttribute 替代 ExecutingLockMode 设置", true)]
        public ResponseCachingAttribute(int duration, CacheMode mode, CacheStoreLocation storeLocation, CacheKeyStrictMode strictMode, ExecutingLockMode lockMode) : this(duration)
        {
            Mode = mode;
            StoreLocation = storeLocation;
            StrictMode = strictMode;
            LockMode = lockMode;
        }

        #endregion Public 构造函数
    }
}