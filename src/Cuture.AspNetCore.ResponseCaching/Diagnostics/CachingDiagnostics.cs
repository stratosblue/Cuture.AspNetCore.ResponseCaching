using System;
using System.Diagnostics;

using Cuture.AspNetCore.ResponseCaching.CacheKey.Generators;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cuture.AspNetCore.ResponseCaching.Diagnostics
{
    /// <summary>
    /// 缓存诊断
    /// </summary>
    public class CachingDiagnostics
    {
        #region Public 属性

        /// <inheritdoc cref="System.Diagnostics.DiagnosticSource"/>
        public DiagnosticSource? DiagnosticSource { get; set; }

        /// <inheritdoc cref="ILogger"/>
        public ILogger Logger { get; }

        #endregion Public 属性

        #region Public 构造函数

        /// <summary>
        /// <inheritdoc cref="CachingDiagnostics"/>
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="diagnosticSource"></param>
        public CachingDiagnostics(IServiceProvider serviceProvider, DiagnosticSource? diagnosticSource = null)
        {
            Logger = serviceProvider.GetRequiredService<ILogger<CachingDiagnostics>>();
            DiagnosticSource = diagnosticSource;
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc cref="CacheBodyTooLargeEventData"/>
        public void CacheBodyTooLarge(string key, ReadOnlyMemory<byte> body, int maxAvailableLength, FilterContext filterContext, object context)
        {
            if (DiagnosticSource != null
                && DiagnosticSource.IsEnabled(CacheBodyTooLargeEventData.EventName))
            {
                DiagnosticSource.Write(CacheBodyTooLargeEventData.EventName, new CacheBodyTooLargeEventData(key, body, maxAvailableLength, filterContext, context));
                return;
            }
            Logger.LogWarning("Response too long to cache, key: {0}, maxLength: {1}, length: {2}", key, maxAvailableLength, body.Length);
        }

        /// <inheritdoc cref="CacheKeyGeneratedEventData"/>
        public void CacheKeyGenerated(FilterContext filterContext, string key, ICacheKeyGenerator keyGenerator, object context)
        {
            if (DiagnosticSource != null
                && DiagnosticSource.IsEnabled(CacheKeyGeneratedEventData.EventName))
            {
                DiagnosticSource.Write(CacheKeyGeneratedEventData.EventName, new CacheKeyGeneratedEventData(filterContext, key, keyGenerator, context));
            }
        }

        /// <inheritdoc cref="CacheKeyTooLongEventData"/>
        public void CacheKeyTooLong(string key, int maxAvailableLength, FilterContext filterContext, object context)
        {
            if (DiagnosticSource != null
                && DiagnosticSource.IsEnabled(CacheKeyTooLongEventData.EventName))
            {
                DiagnosticSource.Write(CacheKeyTooLongEventData.EventName, new CacheKeyTooLongEventData(key, maxAvailableLength, filterContext, context));
                return;
            }
            Logger.LogWarning("CacheKey is too long to cache. maxLength: {0} key: {1}", maxAvailableLength, key);
        }

        /// <inheritdoc cref="CannotExecutionThroughLockEventData"/>
        public void CannotExecutionThroughLock(string key, FilterContext filterContext, object context)
        {
            if (DiagnosticSource != null
                && DiagnosticSource.IsEnabled(CannotExecutionThroughLockEventData.EventName))
            {
                DiagnosticSource.Write(CannotExecutionThroughLockEventData.EventName, new CannotExecutionThroughLockEventData(key, filterContext, context));
            }
        }

        /// <inheritdoc cref="EndProcessingCacheEventData"/>
        public void EndProcessingCache(FilterContext filterContext, object context)
        {
            if (DiagnosticSource != null
                && DiagnosticSource.IsEnabled(EndProcessingCacheEventData.EventName))
            {
                DiagnosticSource.Write(EndProcessingCacheEventData.EventName, new EndProcessingCacheEventData(filterContext, context));
            }
        }

        /// <inheritdoc cref="NoCachingFoundedEventData"/>
        public void NoCachingFounded(string key, FilterContext filterContext, object context)
        {
            if (DiagnosticSource != null
                && DiagnosticSource.IsEnabled(NoCachingFoundedEventData.EventName))
            {
                DiagnosticSource.Write(NoCachingFoundedEventData.EventName, new NoCachingFoundedEventData(key, filterContext, context));
            }
        }

        /// <inheritdoc cref="ResponseFromActionResultEventData"/>
        public void ResponseFromActionResult(ActionExecutingContext executingContext, IActionResult actionResult, object context)
        {
            if (DiagnosticSource != null
                && DiagnosticSource.IsEnabled(ResponseFromActionResultEventData.EventName))
            {
                DiagnosticSource.Write(ResponseFromActionResultEventData.EventName, new ResponseFromActionResultEventData(executingContext, actionResult, context));
            }
        }

        /// <inheritdoc cref="ResponseFromCacheEventData"/>
        public void ResponseFromCache(ActionContext actionContext, ResponseCacheEntry cacheEntry, object context)
        {
            if (DiagnosticSource != null
                && DiagnosticSource.IsEnabled(ResponseFromCacheEventData.EventName))
            {
                DiagnosticSource.Write(ResponseFromCacheEventData.EventName, new ResponseFromCacheEventData(actionContext, cacheEntry, context));
            }
        }

        /// <inheritdoc cref="StartProcessingCacheEventData"/>
        public void StartProcessingCache(FilterContext filterContext, object context)
        {
            if (DiagnosticSource != null
                && DiagnosticSource.IsEnabled(StartProcessingCacheEventData.EventName))
            {
                DiagnosticSource.Write(StartProcessingCacheEventData.EventName, new StartProcessingCacheEventData(filterContext, context));
            }
        }

        #endregion Public 方法
    }
}