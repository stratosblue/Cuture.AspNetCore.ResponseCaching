using System;

using Cuture.AspNetCore.ResponseCaching;
using Cuture.AspNetCore.ResponseCaching.Metadatas;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 指定缓存通行模式（设置执行action的并发控制）<para/>
/// Note:<para/>
/// * 越细粒度的控制会带来相对更多的性能消耗<para/>
/// * 虽然已经尽可能的实现了并发控制，仍然最好不要依赖此功能实现具体业务<para/>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class ExecutingLockAttribute : Attribute, IExecutingLockMetadata
{
    #region Public 属性

    /// <inheritdoc/>
    public int? LockMillisecondsTimeout { get; } = null;

    /// <inheritdoc/>
    public ExecutingLockMode LockMode { get; } = ExecutingLockMode.Default;

    /// <inheritdoc/>
    public ExecutionLockTimeoutFallbackDelegate? OnExecutionLockTimeout { get; set; }

    #endregion Public 属性

    #region Public 构造函数

    /// <inheritdoc cref="ExecutingLockAttribute(ExecutingLockMode, int)"/>
    public ExecutingLockAttribute(ExecutingLockMode lockMode)
    {
        LockMode = Checks.ThrowIfExecutingLockModeIsNone(lockMode);
    }

    /// <summary>
    /// <inheritdoc cref="ExecutingLockAttribute"/>
    /// </summary>
    /// <param name="lockMode">锁定模式</param>
    /// <param name="lockMillisecondsTimeout">锁定的等待超时时间</param>
    public ExecutingLockAttribute(ExecutingLockMode lockMode, int lockMillisecondsTimeout) : this(lockMode)
    {
        LockMillisecondsTimeout = Checks.ThrowIfLockMillisecondsTimeoutInvalid(lockMillisecondsTimeout);
    }

    #endregion Public 构造函数
}