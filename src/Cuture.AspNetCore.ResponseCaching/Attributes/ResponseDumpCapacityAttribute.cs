using System;

using Cuture.AspNetCore.ResponseCaching;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 指定Dump响应时的<see cref="System.IO.MemoryStream"/>初始化大小
    /// <para/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ResponseDumpCapacityAttribute : Attribute, IDumpStreamFactoryProvider
    {
        #region Public 属性

        /// <summary>
        /// 容量
        /// </summary>
        public int Capacity { get; }

        #endregion Public 属性

        #region Public 构造函数

        /// <summary>
        /// <inheritdoc cref="ResponseDumpCapacityAttribute"/>
        /// </summary>
        /// <param name="capacity"><see cref="System.IO.MemoryStream"/>初始化大小</param>
        public ResponseDumpCapacityAttribute(int capacity)
        {
            if (capacity < ResponseCachingConstants.DefaultMinMaxCacheableResponseLength)
            {
                throw new ArgumentOutOfRangeException($"{nameof(capacity)} can not less than {ResponseCachingConstants.DefaultMinMaxCacheableResponseLength}");
            }
            Capacity = capacity;
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public IDumpStreamFactory Create() => new DefaultDumpStreamFactory(Capacity);

        #endregion Public 方法
    }
}