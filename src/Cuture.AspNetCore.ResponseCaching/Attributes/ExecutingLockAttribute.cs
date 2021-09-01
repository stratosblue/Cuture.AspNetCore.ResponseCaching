using System;

using Cuture.AspNetCore.ResponseCaching.Metadatas;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 指定缓存通行模式（设置执行action的并发控制）及使用的执行锁名称（需要与<see cref="ResponseCachingAttribute"/>及其派生特性组合使用）<para/>
    /// Note:<para/>
    /// * 分散使用不同的ExecutingLocker可以减小锁的粒度<para/>
    /// * 越细粒度的控制会带来相对更多的性能消耗<para/>
    /// * 虽然已经尽可能的实现了并发控制，仍然最好不要依赖此功能实现具体业务<para/>
    /// * 默认实现对 ActionFilter 的锁定效果不敢保证100%<para/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExecutingLockAttribute : Attribute, IExecutingLockMetadata
    {
        #region Public 属性

        /// <inheritdoc/>
        public string LockerName { get; }

        /// <inheritdoc/>
        public ExecutingLockMode LockMode { get; } = ExecutingLockMode.Default;

        #endregion Public 属性

        #region Public 构造函数

        /// <inheritdoc cref="ExecutingLockAttribute"/>
        public ExecutingLockAttribute(ExecutingLockMode lockMode) : this("", lockMode)
        {
        }

        /// <summary>
        /// <inheritdoc cref="ExecutingLockAttribute"/>
        /// </summary>
        /// <param name="lockName">locker名称，不同的名称使用不同的locker，默认为 "" </param>
        /// <param name="lockMode">锁定模式</param>
        public ExecutingLockAttribute(string lockName, ExecutingLockMode lockMode)
        {
            if (lockMode == ExecutingLockMode.None)
            {
                throw new ArgumentException("can not be \"ExecutingLockMode.None\"", nameof(lockMode));
            }

            LockerName = lockName ?? string.Empty;
            LockMode = lockMode;
        }

        #endregion Public 构造函数
    }
}