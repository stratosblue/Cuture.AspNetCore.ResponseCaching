using System;

using Cuture.AspNetCore.ResponseCaching;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.Extensions.Configuration;

using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///
/// </summary>
public static class ResponseCachingServicesExtensions
{
    #region Public 方法

    /// <summary>
    /// 使用Redis缓存
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration">The string configuration to use for this multiplexer.</param>
    /// <param name="cacheKeyPrefix">缓存Key前缀</param>
    /// <returns></returns>
    public static ResponseCachingServiceBuilder UseRedisResponseCache(this ResponseCachingServiceBuilder builder, string configuration, string? cacheKeyPrefix = null)
    {
        var connectionMultiplexer = ConnectionMultiplexer.Connect(configuration);
        return builder.UseRedisResponseCache(connectionMultiplexer, cacheKeyPrefix);
    }

    /// <summary>
    /// 使用Redis缓存
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="connectionMultiplexer"></param>
    /// <param name="cacheKeyPrefix">缓存Key前缀</param>
    /// <returns></returns>
    public static ResponseCachingServiceBuilder UseRedisResponseCache(this ResponseCachingServiceBuilder builder, IConnectionMultiplexer connectionMultiplexer, string? cacheKeyPrefix = null)
    {
        return builder.UseRedisResponseCache(options =>
        {
            options.CacheKeyPrefix = cacheKeyPrefix;
            options.ConnectionMultiplexer = connectionMultiplexer;
        });
    }

    /// <summary>
    /// 使用Redis缓存
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static ResponseCachingServiceBuilder UseRedisResponseCache(this ResponseCachingServiceBuilder builder, IConfiguration configuration)
    {
        var connectionConfiguration = configuration.GetValue<string>(nameof(RedisResponseCacheOptions.Configuration));
        if (string.IsNullOrWhiteSpace(connectionConfiguration))
        {
            throw new ArgumentException("Can not find the redis connection string at section 'Configuration'.", nameof(configuration));
        }
        var cacheKeyPrefix = configuration.GetValue<string>(nameof(RedisResponseCacheOptions.CacheKeyPrefix));
        var connectionMultiplexer = ConnectionMultiplexer.Connect(connectionConfiguration);
        return builder.UseRedisResponseCache(connectionMultiplexer, cacheKeyPrefix);
    }

    #endregion Public 方法

    #region Internal 方法

    /// <summary>
    /// 使用Redis缓存
    /// <para/>
    /// 连接redis方法会死锁，所以需要在应用启动时连接
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    internal static ResponseCachingServiceBuilder UseRedisResponseCache(this ResponseCachingServiceBuilder builder, Action<RedisResponseCacheOptions> configureOptions)
    {
        var services = builder.Services;

        services.AddOptions<RedisResponseCacheOptions>().Configure(configureOptions);

        services.AddSingleton<IDistributedResponseCache, RedisResponseCache>();

        return builder;
    }

    #endregion Internal 方法
}
