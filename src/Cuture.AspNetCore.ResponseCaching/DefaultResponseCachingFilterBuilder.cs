using System;

using Cuture.AspNetCore.ResponseCaching.CacheKey.Builders;
using Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;
using Cuture.AspNetCore.ResponseCaching.Diagnostics;
using Cuture.AspNetCore.ResponseCaching.Filters;
using Cuture.AspNetCore.ResponseCaching.Interceptors;
using Cuture.AspNetCore.ResponseCaching.Lockers;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <inheritdoc/>
    internal class DefaultResponseCachingFilterBuilder : IResponseCachingFilterBuilder
    {
        #region Public 方法

        /// <inheritdoc/>
        public IFilterMetadata CreateFilter(IServiceProvider serviceProvider, object context)
        {
            if (context is ResponseCachingAttribute attribute)
            {
            }
            else
            {
                throw new ArgumentException($"{nameof(DefaultResponseCachingFilterBuilder)} only can build filter from {nameof(ResponseCachingAttribute)}", nameof(context));
            }

            var optionsAccessor = serviceProvider.GetRequiredService<IOptions<ResponseCachingOptions>>();
            var options = optionsAccessor.Value;

            if (!options.Enable)
            {
                return EmptyFilterMetadata.Instance;
            }

            CheckDuration(attribute.Duration);

            IResponseCache responseCache = GetResponseCache(serviceProvider, attribute, options);

            ICacheKeyGenerator cacheKeyGenerator = TryGetCustomCacheKeyGenerator(serviceProvider, attribute, out FilterType filterType)
                                                   ?? CreateCacheKeyGenerator(serviceProvider, attribute, options, out filterType);

            var cacheDeterminer = serviceProvider.GetRequiredService<IResponseCacheDeterminer>();

            var cachingDiagnosticsAccessor = serviceProvider.GetRequiredService<CachingDiagnosticsAccessor>();

            var interceptorAggregator = new InterceptorAggregator(GetCachingProcessInterceptor(serviceProvider, attribute));

            var lockMode = attribute.LockMode == ExecutingLockMode.Default ? options.DefaultExecutingLockMode : attribute.LockMode;

            var executingLockerName = GetExecutingLockerName(serviceProvider);

            switch (filterType)
            {
                case FilterType.Resource:
                    {
                        Type? executingLockerType = lockMode switch
                        {
                            ExecutingLockMode.ActionSingle => typeof(IActionSingleResourceExecutingLocker),
                            ExecutingLockMode.CacheKeySingle => typeof(ICacheKeySingleResourceExecutingLocker),
                            _ => null,
                        };
                        var executingLocker = executingLockerType is null
                                                ? null
                                                : serviceProvider.GetRequiredService<IExecutingLockerProvider>().GetLocker<IRequestExecutingLocker<ResourceExecutingContext, ResponseCacheEntry>>(executingLockerType, executingLockerName);
                        var responseCachingContext = new ResponseCachingContext<ResourceExecutingContext, ResponseCacheEntry>(attribute,
                                                                                                                              cacheKeyGenerator,
                                                                                                                              executingLocker!,
                                                                                                                              responseCache,
                                                                                                                              cacheDeterminer,
                                                                                                                              optionsAccessor,
                                                                                                                              interceptorAggregator);
                        return new DefaultResourceCacheFilter(responseCachingContext, cachingDiagnosticsAccessor);
                    }
                case FilterType.Action:
                    {
                        Type? executingLockerType = lockMode switch
                        {
                            ExecutingLockMode.ActionSingle => typeof(IActionSingleActionExecutingLocker),
                            ExecutingLockMode.CacheKeySingle => typeof(ICacheKeySingleActionExecutingLocker),
                            _ => null,
                        };
                        var executingLocker = executingLockerType is null
                                                ? null
                                                : serviceProvider.GetRequiredService<IExecutingLockerProvider>().GetLocker<IRequestExecutingLocker<ActionExecutingContext, IActionResult>>(executingLockerType, executingLockerName);
                        var responseCachingContext = new ResponseCachingContext<ActionExecutingContext, IActionResult>(attribute,
                                                                                                                       cacheKeyGenerator,
                                                                                                                       executingLocker!,
                                                                                                                       responseCache,
                                                                                                                       cacheDeterminer,
                                                                                                                       optionsAccessor,
                                                                                                                       interceptorAggregator);
                        return new DefaultActionCacheFilter(responseCachingContext, cachingDiagnosticsAccessor);
                    }
                default:
                    throw new NotImplementedException($"Not ready to support FilterType: {filterType}");
            }
        }

        #endregion Public 方法

        #region Private 方法

        private static void CheckDuration(int duration)
        {
            if (duration < ResponseCachingConstants.MinCacheAvailableSeconds)
            {
                throw new ArgumentOutOfRangeException($"{nameof(duration)} can not less than {ResponseCachingConstants.MinCacheAvailableSeconds} second");
            }
        }

        /// <summary>
        /// 创建CacheKeyGenerator
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="attribute"></param>
        /// <param name="options"></param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        private static ICacheKeyGenerator CreateCacheKeyGenerator(IServiceProvider serviceProvider, ResponseCachingAttribute attribute, ResponseCachingOptions options, out FilterType filterType)
        {
            var strictMode = attribute.StrictMode == CacheKeyStrictMode.Default ? options.DefaultStrictMode : attribute.StrictMode;
            filterType = FilterType.Resource;

            ICacheKeyGenerator cacheKeyGenerator;
            switch (attribute.Mode)
            {
                case CacheMode.FullPathAndQuery:
                    {
                        cacheKeyGenerator = serviceProvider.GetRequiredService<FullPathAndQueryCacheKeyGenerator>();
                        break;
                    }

                case CacheMode.Custom:
                    {
                        CacheKeyBuilder? keyBuilder = null;
                        if (attribute.VaryByHeaders?.Length > 0)
                        {
                            keyBuilder = new RequestHeadersCacheKeyBuilder(keyBuilder, strictMode, attribute.VaryByHeaders);
                        }
                        if (attribute.VaryByClaims?.Length > 0)
                        {
                            keyBuilder = new ClaimsCacheKeyBuilder(keyBuilder, strictMode, attribute.VaryByClaims);
                        }
                        if (attribute.VaryByQueryKeys?.Length > 0)
                        {
                            keyBuilder = new QueryKeysCacheKeyBuilder(keyBuilder, strictMode, attribute.VaryByQueryKeys);
                        }
                        if (attribute.VaryByFormKeys?.Length > 0)
                        {
                            keyBuilder = new FormKeysCacheKeyBuilder(keyBuilder, strictMode, attribute.VaryByFormKeys);
                        }
                        if (attribute.VaryByModels != null)
                        {
                            var modelKeyParserType = attribute.ModelKeyParserType ?? typeof(DefaultModelKeyParser);
                            var modelKeyParser = serviceProvider.GetRequiredService(modelKeyParserType) as IModelKeyParser;
                            keyBuilder = new ModelCacheKeyBuilder(keyBuilder, strictMode, attribute.VaryByModels, modelKeyParser!);
                            filterType = FilterType.Action;
                        }

                        if (keyBuilder is null)
                        {
                            throw new ArgumentException("Custom CacheMode must has keys than 1");
                        }

                        cacheKeyGenerator = new DefaultCacheKeyGenerator(keyBuilder);
                        break;
                    }

                case CacheMode.PathUniqueness:
                    {
                        cacheKeyGenerator = serviceProvider.GetRequiredService<RequestPathCacheKeyGenerator>();
                        break;
                    }

                default:
                    throw new NotImplementedException($"Not ready to support CacheMode: {attribute.Mode}");
            }

            return cacheKeyGenerator;
        }

        /// <summary>
        /// 获取<see cref="ICachingProcessInterceptor"/>
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private static ICachingProcessInterceptor? GetCachingProcessInterceptor(IServiceProvider serviceProvider, ResponseCachingAttribute attribute)
        {
            var type = attribute.CachingProcessInterceptorType
                            ?? serviceProvider.GetRequiredService<IOptions<InterceptorOptions>>().Value.CachingProcessInterceptorType;

            if (type is null)
            {
                return null;
            }

            var interceptor = serviceProvider.GetRequiredService(type);

            return interceptor as ICachingProcessInterceptor;
        }

        private static string GetExecutingLockerName(IServiceProvider serviceProvider)
        {
            var executingLockerAttribute = GetHttpContextMetadata<ExecutingLockerAttribute>(serviceProvider);
            return executingLockerAttribute?.Name ?? string.Empty;
        }

        private static T? GetHttpContextMetadata<T>(IServiceProvider serviceProvider) where T : class
        {
            var endpoint = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext?.GetEndpoint();
            if (endpoint is null)
            {
                throw new ResponseCachingException("Cannot access Endpoint by IHttpContextAccessor.");
            }
            return endpoint.Metadata.GetMetadata<T>();
        }

        /// <summary>
        /// 获取响应缓存容器
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="attribute"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static IResponseCache GetResponseCache(IServiceProvider serviceProvider, ResponseCachingAttribute attribute, ResponseCachingOptions options)
        {
            var storeLocation = attribute.StoreLocation == CacheStoreLocation.Default
                                                                ? options.DefaultCacheStoreLocation
                                                                : attribute.StoreLocation;

            switch (storeLocation)
            {
                case CacheStoreLocation.Distributed:
                    {
                        var responseCache = serviceProvider.GetRequiredService<IDistributedResponseCache>();
                        var hotDataCacheBuilder = GetHttpContextMetadata<IHotDataCacheBuilder>(serviceProvider);
                        if (hotDataCacheBuilder is not null)
                        {
                            var hotDataCache = hotDataCacheBuilder.Build(serviceProvider);
                            if (hotDataCache is null)
                            {
                                throw new ResponseCachingException($"The data cache {hotDataCacheBuilder.GetType()} provided is null.");
                            }
                            return new ResponseCacheHotDataCacheWrapper(responseCache, hotDataCache);
                        }

                        return responseCache;
                    }
                case CacheStoreLocation.Memory:
                    return serviceProvider.GetRequiredService<IMemoryResponseCache>();

                case CacheStoreLocation.Default:
                default:
                    throw new ArgumentException($"UnSupport cache location {storeLocation}");
            }
        }

        /// <summary>
        /// 尝试获取自定义缓存键生成器
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="attribute"></param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        private static ICacheKeyGenerator? TryGetCustomCacheKeyGenerator(IServiceProvider serviceProvider, ResponseCachingAttribute attribute, out FilterType filterType)
        {
            filterType = FilterType.Resource;
            if (attribute.CustomCacheKeyGeneratorType == null)
            {
                return null;
            }

            if (!typeof(ICustomCacheKeyGenerator).IsAssignableFrom(attribute.CustomCacheKeyGeneratorType))
            {
                throw new ArgumentException($"type of {attribute.CustomCacheKeyGeneratorType} must derives from {nameof(ICustomCacheKeyGenerator)}");
            }

            ICustomCacheKeyGenerator cacheKeyGenerator = (ICustomCacheKeyGenerator)serviceProvider.GetRequiredService(attribute.CustomCacheKeyGeneratorType);
            filterType = cacheKeyGenerator.FilterType;

            if (cacheKeyGenerator is IResponseCachingAttributeSetter responseCachingAttributeSetter)
            {
                responseCachingAttributeSetter.SetResponseCachingAttribute(attribute);
            }

            return cacheKeyGenerator;
        }

        #endregion Private 方法
    }
}