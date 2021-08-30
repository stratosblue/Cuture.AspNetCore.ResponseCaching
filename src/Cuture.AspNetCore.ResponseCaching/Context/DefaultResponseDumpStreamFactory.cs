using System.IO;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 默认 <inheritdoc cref="IResponseDumpStreamFactory"/>
    /// </summary>
    internal class DefaultResponseDumpStreamFactory : IResponseDumpStreamFactory
    {
        #region Public 方法

        /// <inheritdoc/>
        public MemoryStream Create(int initialCapacity) => new(initialCapacity);

        #endregion Public 方法
    }
}