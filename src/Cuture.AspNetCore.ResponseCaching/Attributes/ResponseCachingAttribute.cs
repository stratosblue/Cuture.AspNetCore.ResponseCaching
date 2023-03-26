using System;

using Cuture.AspNetCore.ResponseCaching;
using Cuture.AspNetCore.ResponseCaching.Interceptors;
using Cuture.AspNetCore.ResponseCaching.Metadatas;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 响应缓存
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ResponseCachingAttribute
    : ResponseCacheableAttribute
    , IResponseDurationMetadata
    , IMaxCacheableResponseLengthMetadata
    , ICacheModeMetadata
    , ICacheKeyStrictModeMetadata
    , ICacheStoreLocationMetadata
    , IResponseQueryCachePatternMetadata
    , IResponseModelCachePatternMetadata
    , IResponseFormCachePatternMetadata
    , IResponseHeaderCachePatternMetadata
    , IResponseClaimCachePatternMetadata
{
    #region Private 字段

    private int? _maxCacheableResponseLength;

    #endregion Private 字段

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
    /// </summary>
    [Obsolete("使用 ExecutingLockAttribute 替代此属性", true)]
    public ExecutingLockMode LockMode { get; } = ExecutingLockMode.Default;

    /// <inheritdoc cref="IMaxCacheableResponseLengthMetadata.MaxCacheableResponseLength"/>
    public int MaxCacheableResponseLength { get => _maxCacheableResponseLength ?? 0; set => _maxCacheableResponseLength = value; }

    /// <inheritdoc/>
    public CacheMode Mode { get; set; } = CacheMode.Default;

    /// <inheritdoc/>
    public CacheStoreLocation StoreLocation { get; set; } = CacheStoreLocation.Default;

    /// <inheritdoc/>
    public CacheKeyStrictMode StrictMode { get; set; } = CacheKeyStrictMode.Default;

    /// <inheritdoc/>
    public string[]? VaryByClaims { get; set; }

    /// <inheritdoc/>
    public string[]? VaryByFormKeys { get; set; }

    /// <inheritdoc/>
    public string[]? VaryByHeaders { get; set; }

    /// <inheritdoc/>
    public string[]? VaryByModels { get; set; }

    /// <inheritdoc/>
    public string[]? VaryByQueryKeys { get; set; }

    #region Types

    /// <summary>
    /// 缓存处理拦截器类型
    /// <para/>use custom attribute with <see cref="IResponseCachingInterceptor"/> instead
    /// </summary>
    [Obsolete("Use custom implementation \"CachingProcessInterceptor\" attribute instead.", true)]
    public Type? CachingProcessInterceptorType { get; }

    /// <summary>
    /// 自定义缓存键生成器类型
    /// <para/>use <see cref="CacheKeyGeneratorAttribute"/> instead
    /// </summary>
    [Obsolete("Use \"CacheKeyGeneratorAttribute\" instead.", true)]
    public Type? CustomCacheKeyGeneratorType { get; }

    /// <summary>
    /// Model的Key解析器类型
    /// <para/>use <see cref="CacheModelKeyParserAttribute"/> instead
    /// </summary>
    [Obsolete("Use \"CacheModelKeyParserAttribute\" instead.", true)]
    public Type? ModelKeyParserType { get; }

    #endregion Types

    #endregion Public 属性

    #region interfaces

    /// <inheritdoc/>
    int? IMaxCacheableResponseLengthMetadata.MaxCacheableResponseLength => _maxCacheableResponseLength;

    #endregion interfaces

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
