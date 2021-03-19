using System;

using Cuture.AspNetCore.ResponseCaching.Lockers;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 指定使用的 <see cref="IRequestExecutingLocker{TExecutingContext, TCachingData}"/>
    /// <para/>
    /// 分散ExecutingLocker可以减小锁的粒度
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExecutingLockerAttribute : Attribute
    {
        #region Public 属性

        public string Name { get; set; }

        #endregion Public 属性

        #region Public 构造函数

        /// <summary>
        /// <inheritdoc cref="ExecutingLockerAttribute"/>
        /// </summary>
        /// <param name="name">locker名称</param>
        public ExecutingLockerAttribute(string name)
        {
            Name = name ?? string.Empty;
        }

        #endregion Public 构造函数
    }
}