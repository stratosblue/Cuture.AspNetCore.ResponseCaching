using System;

namespace Cuture.AspNetCore.ResponseCaching.Diagnostics
{
    /// <summary>
    /// 缓存诊断访问器
    /// </summary>
    public sealed class CachingDiagnosticsAccessor
    {
        #region Public 属性

        /// <inheritdoc cref="Diagnostics.CachingDiagnostics"/>
        public CachingDiagnostics CachingDiagnostics { get; }

        #endregion Public 属性

        #region Public 构造函数

        /// <inheritdoc cref="CachingDiagnosticsAccessor"/>
        public CachingDiagnosticsAccessor(CachingDiagnostics cachingDiagnostics)
        {
            CachingDiagnostics = cachingDiagnostics ?? throw new ArgumentNullException(nameof(cachingDiagnostics));
        }

        #endregion Public 构造函数
    }
}