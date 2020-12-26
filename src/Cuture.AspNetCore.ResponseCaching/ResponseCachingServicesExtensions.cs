using System;

using Cuture.AspNetCore.ResponseCaching;
using Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;
using Cuture.AspNetCore.ResponseCaching.Interceptors;
using Cuture.AspNetCore.ResponseCaching.Lockers;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
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
        public static ResponseCachingBuilder AddCaching(this IServiceCollection services)
        {
            services.AddOptions<InterceptorOptions>();

            services.TryAddSingleton<IMemoryResponseCache, DefaultMemoryResponseCache>();

            services.AddSingleton<FullPathAndQueryCacheKeyGenerator>();
            services.AddSingleton<RequestPathCacheKeyGenerator>();
            services.AddSingleton<DefaultModelKeyParser>();

            services.TryAddSingleton<IResponseCacheDeterminer, DefaultResponseCacheDeterminer>();

            services.TryAddTransient<IActionSingleResourceExecutingLocker, DefaultActionSingleResourceExecutingLocker>();
            services.TryAddTransient<ICacheKeySingleResourceExecutingLocker, DefaultResourceExecutingLocker>();
            services.TryAddTransient<IResourceExecutingLocker, DefaultResourceExecutingLocker>();

            services.TryAddTransient<IActionSingleActionExecutingLocker, DefaultActionSingleActionExecutingLocker>();
            services.TryAddTransient<ICacheKeySingleActionExecutingLocker, DefaultActionExecutingLocker>();
            services.TryAddTransient<IActionExecutingLocker, DefaultActionExecutingLocker>();

            services.AddHttpContextAccessor();

            return new ResponseCachingBuilder(services);
        }

        /// <summary>
        /// 添加响应缓存
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static ResponseCachingBuilder AddCaching(this IServiceCollection services, Action<ResponseCachingOptions> configureOptions)
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
        public static ResponseCachingBuilder AddCaching(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<ResponseCachingOptions>().Bind(configuration);
            return services.AddCaching();
        }

        #endregion AddCaching

        #region Interceptor

        /// <summary>
        /// 配置全局拦截器
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static ResponseCachingBuilder ConfigureInterceptor(this ResponseCachingBuilder builder, Action<InterceptorOptions> configureOptions)
        {
            builder.Services.PostConfigure(configureOptions);
            return builder;
        }

        /// <summary>
        /// 使用缓存命中标记响应头（在命中缓存时的响应头中增加标记）
        /// <para/>
        /// Note!!!
        /// <para/>
        /// * 此设置将会覆盖之前对<see cref="InterceptorOptions.CachingProcessInterceptorType"/>的设置
        /// <para/>
        /// * 对<see cref="InterceptorOptions.CachingProcessInterceptorType"/>的重新设置也会使此设置失效
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ResponseCachingBuilder UseCacheHitStampHeader(this ResponseCachingBuilder builder, string key, string value)
        {
            builder.ConfigureInterceptor(options =>
            {
                options.CachingProcessInterceptorType = typeof(CacheHitStampCachingProcessInterceptor);
            });

            var interceptor = new CacheHitStampCachingProcessInterceptor(key, value);

            builder.Services.AddSingleton<CacheHitStampCachingProcessInterceptor>(interceptor);

            return builder;
        }

        #endregion Interceptor
    }
}