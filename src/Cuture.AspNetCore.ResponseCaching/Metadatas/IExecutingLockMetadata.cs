using System.Threading;

using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseCaching.Metadatas
{
    /// <summary>
    /// <inheritdoc cref="IResponseCachingMetadata"/> - 执行时的锁定模式
    /// </summary>
    public interface IExecutingLockMetadata : IResponseCachingMetadata
    {
        #region Public 属性

        /// <summary>
        /// 锁定的超时时间（毫秒）<para/>
        /// null 或大于 -1 的任意值<para/>
        /// null 表示使用默认值<para/>
        /// <see cref="Timeout.Infinite"/>(-1) 为无限等待
        /// </summary>
        int? LockMillisecondsTimeout { get; }

        /// <inheritdoc cref="ExecutingLockMode"/>
        ExecutingLockMode LockMode { get; }

        /// <inheritdoc cref="ExecutionLockTimeoutFallbackDelegate"/>
        ExecutionLockTimeoutFallbackDelegate? OnExecutionLockTimeout { get; }

        #endregion Public 属性
    }
}