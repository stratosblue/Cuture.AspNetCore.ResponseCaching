using System;

namespace Cuture.AspNetCore.ResponseCaching.Diagnostics
{
    /// <summary>
    /// DiagnosticLogger的订阅释放器访问器
    /// </summary>
    public sealed class DiagnosticLoggerSubscriberDisposerAccessor : IDisposable
    {
        #region Private 字段

        private IDisposable? _disposable;

        #endregion Private 字段

        #region Public 属性

        /// <summary>
        /// 释放订阅对象
        /// </summary>
        public IDisposable? Disposable
        {
            get => _disposable;
            set
            {
                _disposable?.Dispose();
                _disposable = value;
            }
        }

        #endregion Public 属性

        #region Public 方法

        /// <inheritdoc/>
        public void Dispose()
        {
            _disposable?.Dispose();
        }

        #endregion Public 方法
    }
}