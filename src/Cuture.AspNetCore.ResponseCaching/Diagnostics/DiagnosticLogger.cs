using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cuture.AspNetCore.ResponseCaching.Diagnostics
{
    /// <summary>
    /// 诊断信息打印器
    /// </summary>
    public sealed class DiagnosticLogger : IObserver<KeyValuePair<string, object>>
    {
        #region Private 字段

        private readonly ILogger _logger;

        #endregion Private 字段

        #region Public 构造函数

        /// <inheritdoc cref="DiagnosticLogger"/>
        public DiagnosticLogger(ILogger<CachingDiagnostics> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc cref="DiagnosticLogger"/>
        public DiagnosticLogger(IServiceProvider serviceProvider) : this(serviceProvider.GetRequiredService<ILogger<CachingDiagnostics>>())
        {
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public void OnCompleted()
        {
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
        }

        /// <inheritdoc/>
        public void OnNext(KeyValuePair<string, object> value)
        {
            switch (value.Key)
            {
                case StartProcessingCacheEventData.EventName:
                    if (value.Value is StartProcessingCacheEventData startProcessingCacheEventData)
                    {
                        _logger.LogInformation("Start process request-\"{0}\" with caching filter", startProcessingCacheEventData.FilterContext.HttpContext.TraceIdentifier);
                    }
                    return;

                case CacheKeyGeneratedEventData.EventName:
                    if (value.Value is CacheKeyGeneratedEventData cacheKeyGeneratedEventData)
                    {
                        _logger.LogInformation("Cache key for request-\"{0}\" has generated, the key is \"{1}\"",
                                               cacheKeyGeneratedEventData.FilterContext.HttpContext.TraceIdentifier,
                                               cacheKeyGeneratedEventData.Key);
                    }
                    return;

                case CacheKeyTooLongEventData.EventName:
                    if (value.Value is CacheKeyTooLongEventData cacheKeyTooLongEventData)
                    {
                        _logger.LogWarning("The cache key \"{0}\" from request-\"{1}\" is too long. the max key length is {2}.",
                                           cacheKeyTooLongEventData.Key,
                                           cacheKeyTooLongEventData.FilterContext.HttpContext.TraceIdentifier,
                                           cacheKeyTooLongEventData.MaxAvailableLength);
                    }
                    return;

                case NoCachingFoundedEventData.EventName:
                    if (value.Value is NoCachingFoundedEventData noCachingFoundedEventData)
                    {
                        _logger.LogInformation("No cache useable for request-\"{0}\" with key \"{1}\".",
                                               noCachingFoundedEventData.FilterContext.HttpContext.TraceIdentifier,
                                               noCachingFoundedEventData.Key);
                    }
                    return;

                case CacheBodyTooLargeEventData.EventName:
                    if (value.Value is CacheBodyTooLargeEventData cacheBodyTooLargeEventData)
                    {
                        _logger.LogWarning("The response body from request-\"{0}\" is too large to cache. the max body size is {1}.",
                                           cacheBodyTooLargeEventData.FilterContext.HttpContext.TraceIdentifier,
                                           cacheBodyTooLargeEventData.MaxAvailableLength);
                    }
                    return;

                case ResponseFromCacheEventData.EventName:
                    if (value.Value is ResponseFromCacheEventData responseFromCacheEventData)
                    {
                        _logger.LogInformation("Request-\"{0}\" has responsed from caching.",
                                               responseFromCacheEventData.ActionContext.HttpContext.TraceIdentifier);
                    }
                    return;

                case ResponseFromActionResultEventData.EventName:
                    if (value.Value is ResponseFromActionResultEventData responseFromActionResultEventData)
                    {
                        _logger.LogInformation("Request-\"{0}\" has responsed from ActionResult directly.",
                                               responseFromActionResultEventData.ActionExecutingContext.HttpContext.TraceIdentifier);
                    }
                    return;

                case EndProcessingCacheEventData.EventName:
                    if (value.Value is EndProcessingCacheEventData endProcessingCacheEventData)
                    {
                        _logger.LogInformation("Request-\"{0}\" caching process end.", endProcessingCacheEventData.FilterContext.HttpContext.TraceIdentifier);
                    }
                    return;
            }
            _logger.LogInformation("Diagnostic Data: {0}", value.ToString());
        }

        #endregion Public 方法
    }
}