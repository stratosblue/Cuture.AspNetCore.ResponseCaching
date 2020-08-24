using System;

using Cuture.AspNetCore.ResponseCaching;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.Extensions.Configuration;

using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ResponseCachingServicesExtensions
    {
        /// <summary>
        /// 使用Redis缓存
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration">The string configuration to use for this multiplexer.</param>
        /// <param name="cacheKeyPrefix">缓存Key前缀</param>
        /// <returns></returns>
        public static ResponseCachingBuilder UseRedisResponseCache(this ResponseCachingBuilder builder, string configuration, string cacheKeyPrefix = null)
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
        public static ResponseCachingBuilder UseRedisResponseCache(this ResponseCachingBuilder builder, IConnectionMultiplexer connectionMultiplexer, string cacheKeyPrefix = null)
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
        public static ResponseCachingBuilder UseRedisResponseCache(this ResponseCachingBuilder builder, IConfiguration configuration)
        {
            var connectionConfiguration = configuration.GetValue<string>(nameof(RedisResponseCacheOptions.Configuration));
            var connectionMultiplexer = ConnectionMultiplexer.Connect(connectionConfiguration);
            return builder.UseRedisResponseCache(connectionMultiplexer);
        }

        /// <summary>
        /// 使用Redis缓存
        /// <para/>
        /// 连接redis方法会死锁，所以需要在应用启动时连接
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        internal static ResponseCachingBuilder UseRedisResponseCache(this ResponseCachingBuilder builder, Action<RedisResponseCacheOptions> configureOptions)
        {
            var services = builder.Services;

            services.AddOptions<RedisResponseCacheOptions>().Configure(configureOptions);

            services.AddSingleton<IDistributedResponseCache, RedisResponseCache>();

            return builder;
        }
    }
}