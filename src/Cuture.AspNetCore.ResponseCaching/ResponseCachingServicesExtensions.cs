using System;

using Cuture.AspNetCore.ResponseCaching;
using Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;
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
        /// <summary>
        /// 添加响应缓存
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static ResponseCachingBuilder AddCaching(this IServiceCollection services)
        {
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
    }
}