﻿using Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;
using Cuture.AspNetCore.ResponseCaching.Interceptors;
using Cuture.AspNetCore.ResponseCaching.Internal;
using Cuture.AspNetCore.ResponseCaching.Metadatas;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseCaching;

/// <summary>
/// 响应缓存上下文
/// </summary>
public class ResponseCachingContext : IDisposable
{
    #region Private 字段

    private bool _disposedValue;

    #endregion Private 字段

    #region Public 属性

    /// <summary>
    /// 响应缓存确定器
    /// </summary>
    public IResponseCacheDeterminer CacheDeterminer { get; set; }

    /// <summary>
    /// 用于Dump的MemoryStream初始化大小
    /// </summary>
    public int DumpStreamCapacity { get; }

    /// <summary>
    /// 缓存有效时长（秒）
    /// </summary>
    public int Duration { get; }

    /// <summary>
    /// 缓存有效时长（毫秒）
    /// </summary>
    public long DurationMilliseconds { get; }

    /// <summary>
    /// 拦截器集合
    /// </summary>
    public InterceptorAggregator Interceptors { get; }

    /// <summary>
    /// 缓存Key生成器
    /// </summary>
    public ICacheKeyGenerator KeyGenerator { get; }

    /// <summary>
    /// 锁定的等待超时时间
    /// </summary>
    public int LockMillisecondsTimeout { get; }

    /// <summary>
    /// 最大可缓存响应长度
    /// </summary>
    public int MaxCacheableResponseLength { get; } = -1;

    /// <summary>
    /// 缓存Key的最大长度
    /// </summary>
    public int MaxCacheKeyLength { get; }

    /// <summary>
    /// 无法使用锁执行请求时（Semaphore池用尽）的回调
    /// </summary>
    public CannotExecutionThroughLockDelegate OnCannotExecutionThroughLock { get; }

    /// <summary>
    /// 执行锁定超时的回调
    /// </summary>
    public ExecutionLockTimeoutFallbackDelegate OnExecutionLockTimeout { get; }

    /// <summary>
    /// 响应缓存容器
    /// </summary>
    public IResponseCache ResponseCache { get; }

    /// <summary>
    /// 构建时使用的 <see cref="IServiceProvider"/>
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    #endregion Public 属性

    #region Protected 构造函数

    /// <inheritdoc cref="ResponseCachingContext"/>
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

    protected ResponseCachingContext()
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    {
    }

    #endregion Protected 构造函数

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="ResponseCachingContext"/>
    /// </summary>
    /// <param name="metadatas">Action端点的 <see cref="EndpointMetadataCollection"/></param>
    /// <param name="cacheKeyGenerator"></param>
    /// <param name="responseCache"></param>
    /// <param name="cacheDeterminer"></param>
    /// <param name="options"></param>
    /// <param name="interceptorAggregator"></param>
    /// <param name="dumpStreamCapacity"></param>
    /// <param name="serviceProvider"></param>
    public ResponseCachingContext(EndpointMetadataCollection metadatas,
                                  ICacheKeyGenerator cacheKeyGenerator,
                                  IResponseCache responseCache,
                                  IResponseCacheDeterminer cacheDeterminer,
                                  ResponseCachingOptions options,
                                  InterceptorAggregator interceptorAggregator,
                                  int dumpStreamCapacity,
                                  IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(metadatas);

        MaxCacheableResponseLength = Checks.ThrowIfMaxCacheableResponseLengthTooSmall(Metadata<IMaxCacheableResponseLengthMetadata>()?.MaxCacheableResponseLength ?? options.MaxCacheableResponseLength);

        MaxCacheKeyLength = options.MaxCacheKeyLength;

        KeyGenerator = cacheKeyGenerator ?? throw new ArgumentNullException(nameof(cacheKeyGenerator));
        ResponseCache = responseCache ?? throw new ArgumentNullException(nameof(responseCache));
        CacheDeterminer = cacheDeterminer ?? throw new ArgumentNullException(nameof(cacheDeterminer));
        Duration = Checks.ThrowIfDurationTooSmall(RequiredMetadata<IResponseDurationMetadata>().Duration);
        DurationMilliseconds = Duration * 1000;

        var executingLockMetadata = Metadata<IExecutingLockMetadata>();
        LockMillisecondsTimeout = Checks.ThrowIfLockMillisecondsTimeoutInvalid(executingLockMetadata?.LockMillisecondsTimeout ?? options.DefaultLockMillisecondsTimeout).Value;

        Interceptors = interceptorAggregator;

        DumpStreamCapacity = Checks.ThrowIfDumpStreamInitialCapacityTooSmall(dumpStreamCapacity, nameof(dumpStreamCapacity));

        OnCannotExecutionThroughLock = options.OnCannotExecutionThroughLock ?? DefaultExecutionLockTimeoutFallback.SetStatus429;
        OnExecutionLockTimeout = executingLockMetadata?.OnExecutionLockTimeout
                                 ?? options.OnExecutionLockTimeoutFallback
                                 ?? DefaultExecutionLockTimeoutFallback.SetStatus429;

        ServiceProvider = serviceProvider;

        TMetadata? Metadata<TMetadata>() where TMetadata : class => metadatas.GetMetadata<TMetadata>();
        TMetadata RequiredMetadata<TMetadata>() where TMetadata : class => metadatas.RequiredMetadata<TMetadata>();
    }

    #endregion Public 构造函数

    #region Dispose

    /// <summary>
    ///
    /// </summary>
    ~ResponseCachingContext()
    {
        Dispose(disposing: false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _disposedValue = true;
        }
    }

    #endregion Dispose
}
