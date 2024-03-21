using System.Diagnostics;

using Cuture.AspNetCore.ResponseCaching;
using Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;
using Cuture.AspNetCore.ResponseCaching.Diagnostics;
using Cuture.AspNetCore.ResponseCaching.ExecutingLock;
using Cuture.AspNetCore.ResponseCaching.Interceptors;
using Cuture.AspNetCore.ResponseCaching.Internal;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///
/// </summary>
public static class ResponseCachingServicesExtensions
{
    #region AddCaching

    /// <summary>
    /// 添加响应缓存
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static ResponseCachingServiceBuilder AddCaching(this IServiceCollection services)
    {
        services.AddOptions<InterceptorOptions>();

        services.AddOptions<ResponseCachingExecutingLockOptions>();

        services.AddHttpContextAccessor();

        services.TryAddScoped<IEndpointAccessor, DefaultEndpointAccessor>();

        services.TryAddSingleton<IResponseCachingFilterBuilder, DefaultResponseCachingFilterBuilder>();

        services.TryAddSingleton<IMemoryResponseCache, DefaultMemoryResponseCache>();

        services.TryAddSingleton<FullPathAndQueryCacheKeyGenerator>();
        services.TryAddSingleton<RequestPathCacheKeyGenerator>();
        services.TryAddSingleton<DefaultModelKeyParser>();

        services.TryAddSingleton<IResponseCacheDeterminer, DefaultResponseCacheDeterminer>();

        services.AddExecutingLockPool();

        services.TryAddSingleton(serviceProvider => new CachingDiagnostics(serviceProvider));
        services.TryAddSingleton<CachingDiagnosticsAccessor>();

        services.TryAddSingleton<IHotDataCacheProvider, DefaultHotDataCacheProvider>();

        services.TryAddSingleton<IResponseDumpStreamFactory, DefaultResponseDumpStreamFactory>();

        return new ResponseCachingServiceBuilder(services);
    }

    /// <summary>
    /// 添加响应缓存
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static ResponseCachingServiceBuilder AddCaching(this IServiceCollection services, Action<ResponseCachingOptions> configureOptions)
    {
        services.AddOptions<ResponseCachingOptions>().PostConfigure(configureOptions);
        return services.AddCaching();
    }

    /// <summary>
    /// 添加响应缓存
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static ResponseCachingServiceBuilder AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ResponseCachingOptions>().Bind(configuration);
        return services.AddCaching();
    }

    /// <summary>
    /// 添加<see cref="IExecutingLockPool{TCachePayload}"/>
    /// </summary>
    /// <param name="services"></param>
    private static void AddExecutingLockPool(this IServiceCollection services)
    {
        services.TryAddSingleton(services =>
        {
            var options = services.GetRequiredService<IOptions<ResponseCachingExecutingLockOptions>>().Value;

            var boundedObjectPoolOptions = new BoundedObjectPoolOptions()
            {
                MaximumPooled = options.MaximumSemaphorePooled,
                MinimumRetained = options.MinimumSemaphoreRetained,
                RecycleInterval = options.SemaphoreRecycleInterval,
            };

            var semaphorePool = BoundedObjectPool.Create(new SinglePassSemaphoreLifecycleExecutor(), boundedObjectPoolOptions);

            return (INakedBoundedObjectPool<SemaphoreSlim>)semaphorePool;
        });

        services.TryAddSingleton(typeof(ExclusiveExecutingLockLifecycleExecutor<>));
        services.TryAddSingleton(typeof(SharedExecutingLockLifecycleExecutor<>));

        services.TryAddSingleton(serviceProvider => CreateSharedExecutingLockPool(serviceProvider));
        services.TryAddSingleton(serviceProvider => CreateExclusiveExecutingLockPool(serviceProvider));

        static INakedBoundedObjectPool<SharedExecutingLock<ResponseCacheEntry>> CreateSharedExecutingLockPool(IServiceProvider serviceProvider)
        {
            var objectPoolOptions = GetObjectPoolOptions(serviceProvider);

            var lifecycleExecutor = serviceProvider.GetRequiredService<SharedExecutingLockLifecycleExecutor<ResponseCacheEntry>>();
            var pool = BoundedObjectPool.Create(lifecycleExecutor, objectPoolOptions);
            return (INakedBoundedObjectPool<SharedExecutingLock<ResponseCacheEntry>>)pool;
        }

        static INakedBoundedObjectPool<ExclusiveExecutingLock<ResponseCacheEntry>> CreateExclusiveExecutingLockPool(IServiceProvider serviceProvider)
        {
            var objectPoolOptions = GetObjectPoolOptions(serviceProvider);

            var lifecycleExecutor = serviceProvider.GetRequiredService<ExclusiveExecutingLockLifecycleExecutor<ResponseCacheEntry>>();
            var pool = BoundedObjectPool.Create(lifecycleExecutor, objectPoolOptions);
            return (INakedBoundedObjectPool<ExclusiveExecutingLock<ResponseCacheEntry>>)pool;
        }

        static BoundedObjectPoolOptions GetObjectPoolOptions(IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetRequiredService<IOptions<ResponseCachingExecutingLockOptions>>().Value;

            var boundedObjectPoolOptions = new BoundedObjectPoolOptions()
            {
                MaximumPooled = options.MaximumExecutingLockPooled,
                MinimumRetained = options.MinimumExecutingLockRetained,
                RecycleInterval = options.ExecutingLockRecycleInterval,
            };
            return boundedObjectPoolOptions;
        }
    }

    #endregion AddCaching

    #region Interceptor

    /// <summary>
    /// 配置全局拦截器
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static ResponseCachingServiceBuilder ConfigureInterceptor(this ResponseCachingServiceBuilder builder, Action<InterceptorOptions> configureOptions)
    {
        builder.Services.PostConfigure(configureOptions);
        return builder;
    }

    /// <summary>
    /// 使用缓存命中标记响应头（在命中缓存时的响应头中增加标记）
    /// <para/>
    /// * 此设置可能因为拦截器短路而不执行
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ResponseCachingServiceBuilder UseCacheHitStampHeader(this ResponseCachingServiceBuilder builder, string key, string value)
    {
        builder.ConfigureInterceptor(options =>
        {
            options.AddInterceptor(new CacheHitStampInterceptor(key, value));
        });

        return builder;
    }

    #endregion Interceptor

    #region Diagnostics

    #region services

    /// <summary>
    /// 添加Debug模式下的诊断信息日志输出
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static ResponseCachingServiceBuilder AddDiagnosticDebugLogger(this ResponseCachingServiceBuilder builder)
    {
        builder.InternalAddDiagnosticDebugLogger();
        return builder;
    }

    /// <summary>
    /// 添加Release模式下的诊断信息日志输出
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static ResponseCachingServiceBuilder AddDiagnosticReleaseLogger(this ResponseCachingServiceBuilder builder)
    {
        builder.InternalAddDiagnosticReleaseLogger();
        return builder;
    }

    /// <summary>
    /// 启用 <see cref="ICacheKeyAccessor"/>
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static ResponseCachingServiceBuilder EnableCacheKeyAccessor(this ResponseCachingServiceBuilder builder)
    {
        builder.Services.TryAddSingleton<CacheKeyAccessor>();
        builder.Services.TryAddSingleton<ICacheKeyAccessor>(static serviceProvider => serviceProvider.GetRequiredService<CacheKeyAccessor>());

        return builder;
    }

    [Conditional("DEBUG")]
    internal static void InternalAddDiagnosticDebugLogger(this ResponseCachingServiceBuilder builder)
    {
        builder.InternalAddDiagnosticLogger();
    }

    internal static void InternalAddDiagnosticLogger(this ResponseCachingServiceBuilder builder)
    {
        var services = builder.Services;

        var diagnosticsDescriptor = ServiceDescriptor.Singleton(serviceProvider => new CachingDiagnostics(serviceProvider, new DiagnosticListener(ResponseCachingEventData.DiagnosticName)));
        services.Replace(diagnosticsDescriptor);

        services.TryAddSingleton(serviceProvider => new DiagnosticLogger(serviceProvider));
        services.TryAddSingleton(serviceProvider => new DiagnosticLoggerSubscriber(serviceProvider));
        services.TryAddSingleton(_ => new DiagnosticLoggerSubscriberDisposerAccessor());
    }

    [Conditional("RELEASE")]
    internal static void InternalAddDiagnosticReleaseLogger(this ResponseCachingServiceBuilder builder)
    {
        builder.InternalAddDiagnosticLogger();
    }

    #endregion services

    #region initialization

    /// <summary>
    /// 启用缓存诊断日志
    /// </summary>
    /// <param name="builder"></param>
    public static void EnableResponseCachingDiagnosticLogger(this IApplicationBuilder builder)
    {
        var serviceProvider = builder.ApplicationServices;
        var disposable = serviceProvider.EnableResponseCachingDiagnosticLogger();
        if (disposable is not null)
        {
            serviceProvider.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping.Register(() =>
            {
                disposable.Dispose();
            });
        }
    }

    /// <summary>
    /// 启用缓存诊断日志
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static IDisposable? EnableResponseCachingDiagnosticLogger(this IServiceProvider serviceProvider)
    {
        var diagnosticLoggerSubscriber = serviceProvider.GetService<DiagnosticLoggerSubscriber>();
        var diagnosticLoggerSubscriberDisposerAccessor = serviceProvider.GetService<DiagnosticLoggerSubscriberDisposerAccessor>();

        if (diagnosticLoggerSubscriber is null
            || diagnosticLoggerSubscriberDisposerAccessor is null)
        {
            return null;
            //throw new ResponseCachingException($"Must Add Diagnostic Logger Into {nameof(IServiceCollection)} Before Enable ResponseCaching Diagnostic Logger.");
        }

        var disposable = DiagnosticListener.AllListeners.Subscribe(diagnosticLoggerSubscriber);

        diagnosticLoggerSubscriberDisposerAccessor.Disposable = disposable;

        return disposable;
    }

    /// <summary>
    /// 关闭缓存诊断日志
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static void ShutdownResponseCachingDiagnosticLogger(this IServiceProvider serviceProvider)
    {
        serviceProvider.GetService<DiagnosticLoggerSubscriberDisposerAccessor>()?.Dispose();
    }

    #endregion initialization

    #endregion Diagnostics
}
