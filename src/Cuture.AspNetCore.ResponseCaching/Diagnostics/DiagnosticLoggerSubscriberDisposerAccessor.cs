using System;

namespace Cuture.AspNetCore.ResponseCaching.Diagnostics;

/// <summary>
/// DiagnosticLogger的订阅释放器访问器
/// </summary>
public sealed class DiagnosticLoggerSubscriberDisposerAccessor : IDisposable
{
    #region Private 字段

    private IDisposable? _disposable;

    private bool _isDisposed = false;

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
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(DiagnosticLoggerSubscriberDisposerAccessor));
            }
            _disposable?.Dispose();
            _disposable = value;
        }
    }

    #endregion Public 属性

    #region Private 析构函数

    /// <summary>
    ///
    /// </summary>
    ~DiagnosticLoggerSubscriberDisposerAccessor()
    {
        Dispose();
    }

    #endregion Private 析构函数

    #region Public 方法

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;
            if (_disposable is not null)
            {
                try
                {
                    _disposable.Dispose();
                }
                catch { }
            }
            GC.SuppressFinalize(this);
        }
    }

    #endregion Public 方法
}