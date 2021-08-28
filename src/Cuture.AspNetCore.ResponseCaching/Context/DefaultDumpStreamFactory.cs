using System;
using System.IO;
using System.Threading;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 默认转储Stream工厂
    /// </summary>
    internal class DefaultDumpStreamFactory : IDumpStreamFactory
    {
        #region Private 字段

        private static readonly Lazy<DefaultDumpStreamFactory> s_defaultShared = new(() => new(ResponseCachingConstants.DefaultDumpCapacity), LazyThreadSafetyMode.PublicationOnly);

        #endregion Private 字段

        #region Public 属性

        public static DefaultDumpStreamFactory DefaultShared => s_defaultShared.Value;

        /// <summary>
        /// 初始化 <see cref="MemoryStream"/> 的容量
        /// </summary>
        public int InitialCapacity { get; }

        #endregion Public 属性

        #region Public 构造函数

        /// <summary>
        /// 默认转储Stream工厂
        /// </summary>
        /// <param name="initialCapacity"></param>
        public DefaultDumpStreamFactory(int initialCapacity)
        {
            InitialCapacity = initialCapacity;
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public MemoryStream Create() => new(InitialCapacity);

        #endregion Public 方法
    }
}