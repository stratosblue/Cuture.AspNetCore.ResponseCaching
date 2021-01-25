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

        private readonly ILogger<CachingDiagnostics> _logger;

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
            _logger.LogWarning(value.ToString());
        }

        #endregion Public 方法
    }
}