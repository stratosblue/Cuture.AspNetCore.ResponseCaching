using System;
using System.Collections.Generic;
using System.Linq;

using Cuture.AspNetCore.ResponseCaching.CacheKey.Builders;
using Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;
using Cuture.AspNetCore.ResponseCaching.Diagnostics;
using Cuture.AspNetCore.ResponseCaching.Filters;
using Cuture.AspNetCore.ResponseCaching.Interceptors;
using Cuture.AspNetCore.ResponseCaching.Internal;
using Cuture.AspNetCore.ResponseCaching.Lockers;
using Cuture.AspNetCore.ResponseCaching.Metadatas;
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
        public IFilterMetadata CreateFilter(IServiceProvider serviceProvider, Endpoint endpoint)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (endpoint is null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            var buildContext = new FilterBuildContext(serviceProvider, endpoint);

            if (!buildContext.Options.Enable)
            {
                return EmptyFilterMetadata.Instance;
            }

            IResponseCache responseCache = GetResponseCache(buildContext);

            ICacheKeyGenerator cacheKeyGenerator = TryGetCustomCacheKeyGenerator(buildContext, out FilterType filterType)
                                                   ?? CreateCacheKeyGenerator(buildContext, out filterType);

            var cacheDeterminer = serviceProvider.GetRequiredService<IResponseCacheDeterminer>();

            var cachingDiagnosticsAccessor = serviceProvider.GetRequiredService<CachingDiagnosticsAccessor>();

            var interceptorAggregator = new InterceptorAggregator(GetCachingProcessInterceptor(buildContext));

            var executingLockMetadata = buildContext.GetMetadata<IExecutingLockMetadata>();

            var lockMode = Checks.ThrowIfExecutingLockModeIsNone(executingLockMetadata?.LockMode ?? buildContext.Options.DefaultExecutingLockMode);

            var executingLockerName = executingLockMetadata?.LockerName ?? string.Empty;

            var dumpStreamCapacity = buildContext.GetMetadata<IResponseDumpCapacityMetadata>()?.Capacity
                                        ?? ResponseCachingConstants.DefaultDumpCapacity;

            Checks.ThrowIfDumpCapacityTooSmall(dumpStreamCapacity, nameof(dumpStreamCapacity));

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
                        var responseCachingContext = new ResponseCachingContext<ResourceExecutingContext, ResponseCacheEntry>(metadatas: buildContext.Endpoint.Metadata,
                                                                                                                              cacheKeyGenerator: cacheKeyGenerator,
                                                                                                                              executingLocker: executingLocker!,
                                                                                                                              responseCache: responseCache,
                                                                                                                              cacheDeterminer: cacheDeterminer,
                                                                                                                              options: buildContext.Options,
                                                                                                                              interceptorAggregator: interceptorAggregator,
                                                                                                                              dumpStreamCapacity: dumpStreamCapacity);
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
                        var responseCachingContext = new ResponseCachingContext<ActionExecutingContext, IActionResult>(metadatas: buildContext.Endpoint.Metadata,
                                                                                                                       cacheKeyGenerator: cacheKeyGenerator,
                                                                                                                       executingLocker: executingLocker!,
                                                                                                                       responseCache: responseCache,
                                                                                                                       cacheDeterminer: cacheDeterminer,
                                                                                                                       options: buildContext.Options,
                                                                                                                       interceptorAggregator: interceptorAggregator,
                                                                                                                       dumpStreamCapacity: dumpStreamCapacity);
                        return new DefaultActionCacheFilter(responseCachingContext, cachingDiagnosticsAccessor);
                    }
                default:
                    throw new NotImplementedException($"Not ready to support FilterType: {filterType}");
            }
        }

        #endregion Public 方法

        #region Private 方法

        /// <summary>
        /// 创建CacheKeyGenerator
        /// </summary>
        /// <param name="context"></param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        private static ICacheKeyGenerator CreateCacheKeyGenerator(FilterBuildContext context, out FilterType filterType)
        {
            var cacheMode = context.RequiredMetadata<IResponseCacheModeMetadata>().Mode;

            filterType = FilterType.Resource;

            ICacheKeyGenerator cacheKeyGenerator;
            switch (cacheMode)
            {
                case CacheMode.FullPathAndQuery:
                    {
                        cacheKeyGenerator = context.GetRequiredService<FullPathAndQueryCacheKeyGenerator>();
                        break;
                    }

                case CacheMode.Custom:
                    {
                        var strictMode = context.GetMetadata<IResponseCacheKeyStrictModeMetadata>()?.StrictMode
                                         ?? CacheKeyStrictMode.Default;
                        if (strictMode == CacheKeyStrictMode.Default)
                        {
                            strictMode = context.Options.DefaultStrictMode;
                        }

                        CacheKeyBuilder? keyBuilder = null;
                        if (Metadata<IResponseHeaderCachePatternMetadata>()?.VaryByHeaders is string[] varyByHeaders
                            && varyByHeaders.Length > 0)
                        {
                            keyBuilder = new RequestHeadersCacheKeyBuilder(keyBuilder, strictMode, varyByHeaders);
                        }
                        if (Metadata<IResponseClaimCachePatternMetadata>()?.VaryByClaims is string[] varyByClaims
                            && varyByClaims.Length > 0)
                        {
                            keyBuilder = new ClaimsCacheKeyBuilder(keyBuilder, strictMode, varyByClaims);
                        }
                        if (Metadata<IResponseQueryCachePatternMetadata>()?.VaryByQueryKeys is string[] varyByQueryKeys
                            && varyByQueryKeys.Length > 0)
                        {
                            keyBuilder = new QueryKeysCacheKeyBuilder(keyBuilder, strictMode, varyByQueryKeys);
                        }
                        if (Metadata<IResponseFormCachePatternMetadata>()?.VaryByFormKeys is string[] varyByFormKeys
                            && varyByFormKeys.Length > 0)
                        {
                            keyBuilder = new FormKeysCacheKeyBuilder(keyBuilder, strictMode, varyByFormKeys);
                        }
                        if (Metadata<IResponseModelCachePatternMetadata>()?.VaryByModels is string[] varyByModels)
                        {
                            var modelKeyParserType = context.GetMetadata<ICacheModelKeyParserMetadata>()?.ModelKeyParserType ?? typeof(DefaultModelKeyParser);

                            Checks.ThrowIfNotIModelKeyParser(modelKeyParserType);

                            var modelKeyParser = context.GetRequiredService<IModelKeyParser>(modelKeyParserType);
                            keyBuilder = new ModelCacheKeyBuilder(keyBuilder, strictMode, varyByModels, modelKeyParser);
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
                        cacheKeyGenerator = context.GetRequiredService<RequestPathCacheKeyGenerator>();
                        break;
                    }

                default:
                    throw new NotImplementedException($"Not ready to support CacheMode: {cacheMode}");
            }

            return cacheKeyGenerator;

            TMetadata? Metadata<TMetadata>() where TMetadata : class => context.GetMetadata<TMetadata>();
        }

        /// <summary>
        /// 获取<see cref="ICachingProcessInterceptor"/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static ICachingProcessInterceptor[] GetCachingProcessInterceptor(FilterBuildContext context)
        {
            var options = context.GetRequiredService<IOptions<InterceptorOptions>>().Value;
            var optionInterceptorTypes = options.CachingProcessInterceptorTypes;
            var interceptorTypes = new List<Type>(optionInterceptorTypes);

            var attributeInterceptors = context.Endpoint.Metadata.OfType<ICachingProcessInterceptor>();
            return optionInterceptorTypes.Select(context.GetRequiredService<ICachingProcessInterceptor>)
                                         .Concat(options.CachingProcessInterceptors)
                                         .Concat(attributeInterceptors)
                                         .ToArray();
        }

        /// <summary>
        /// 获取响应缓存容器
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static IResponseCache GetResponseCache(FilterBuildContext context)
        {
            var storeLocation = context.GetMetadata<IResponseCacheStoreLocationMetadata>()?.StoreLocation
                                ?? CacheStoreLocation.Default;

            if (storeLocation == CacheStoreLocation.Default)
            {
                storeLocation = context.Options.DefaultCacheStoreLocation;
            }

            switch (storeLocation)
            {
                case CacheStoreLocation.Distributed:
                    {
                        var responseCache = context.GetRequiredService<IDistributedResponseCache>();
                        var hotDataCacheBuilder = context.GetMetadata<IHotDataCacheBuilder>();
                        if (hotDataCacheBuilder is not null)
                        {
                            var metadata = context.GetMetadata<IHotDataCacheMetadata>() ?? throw new ResponseCachingException($"No {nameof(IHotDataCacheMetadata)} found for {context.Endpoint}");

                            var hotDataCache = hotDataCacheBuilder.Build(context.ServiceProvider, metadata);
                            if (hotDataCache is null)
                            {
                                throw new ResponseCachingException($"The data cache {hotDataCacheBuilder.GetType()} provided is null.");
                            }
                            return new ResponseCacheHotDataCacheWrapper(responseCache, hotDataCache);
                        }

                        return responseCache;
                    }
                case CacheStoreLocation.Memory:
                    return context.GetRequiredService<IMemoryResponseCache>();

                case CacheStoreLocation.Default:
                default:
                    throw new ArgumentException($"UnSupport cache location {storeLocation}");
            }
        }

        /// <summary>
        /// 尝试获取自定义缓存键生成器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        private static ICacheKeyGenerator? TryGetCustomCacheKeyGenerator(FilterBuildContext context, out FilterType filterType)
        {
            filterType = FilterType.Resource;

            var metadata = context.GetMetadata<ICacheKeyGeneratorMetadata>();
            if (metadata is null)
            {
                return null;
            }

            Checks.ThrowIfNotICacheKeyGenerator(metadata.CacheKeyGeneratorType);

            var cacheKeyGenerator = context.GetRequiredService<ICacheKeyGenerator>(metadata.CacheKeyGeneratorType);
            filterType = metadata.FilterType;

            if (cacheKeyGenerator is IEndpointSetter setter)
            {
                setter.SetEndpoint(context.Endpoint);
            }

            return cacheKeyGenerator;
        }

        #endregion Private 方法
    }

    internal class FilterBuildContext
    {
        #region Public 属性

        public Endpoint Endpoint { get; }

        public ResponseCachingOptions Options { get; }

        public IServiceProvider ServiceProvider { get; }

        #endregion Public 属性

        #region Public 构造函数

        public FilterBuildContext(IServiceProvider serviceProvider, Endpoint endpoint)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));

            Options = serviceProvider.GetRequiredService<IOptions<ResponseCachingOptions>>().Value;
        }

        #endregion Public 构造函数

        #region Public 方法

        public T? GetMetadata<T>() where T : class => Endpoint.Metadata.GetMetadata<T>();

        public T GetRequiredService<T>() where T : notnull => ServiceProvider.GetRequiredService<T>();

        public T GetRequiredService<T>(Type type) => (T)ServiceProvider.GetRequiredService(type);

        public T RequiredMetadata<T>() where T : class => Endpoint.Metadata.RequiredMetadata<T>();

        #endregion Public 方法
    }
}