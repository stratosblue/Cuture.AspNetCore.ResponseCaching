using System;
using System.Collections.Concurrent;
using System.Diagnostics;

using Microsoft.Extensions.DependencyInjection;

namespace Cuture.AspNetCore.ResponseCaching.Diagnostics
{
    /// <summary>
    /// 诊断信息日志订阅器
    /// </summary>
    public sealed class DiagnosticLoggerSubscriber : IObserver<DiagnosticListener>
    {
        #region Private 字段

        private readonly WeakReference<DiagnosticLogger> _diagnosticLogger;
        private readonly ConcurrentBag<IDisposable> _disposables = new ConcurrentBag<IDisposable>();

        #endregion Private 字段

        #region Public 构造函数

        /// <inheritdoc cref="DiagnosticLoggerSubscriber"/>
        public DiagnosticLoggerSubscriber(IServiceProvider serviceProvider)
        {
            _diagnosticLogger = new WeakReference<DiagnosticLogger>(serviceProvider.GetRequiredService<DiagnosticLogger>());
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public void OnCompleted()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
        }

        /// <inheritdoc/>
        public void OnNext(DiagnosticListener value)
        {
            if (value.Name.StartsWith(ResponseCachingEventData.DiagnosticName, StringComparison.OrdinalIgnoreCase)
                && _diagnosticLogger.TryGetTarget(out var diagnosticLogger))
            {
                _disposables.Add(value.Subscribe(diagnosticLogger!));
            }
        }

        #endregion Public 方法
    }
}