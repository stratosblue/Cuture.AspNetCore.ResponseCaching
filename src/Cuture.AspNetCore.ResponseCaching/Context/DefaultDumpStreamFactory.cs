using System.IO;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 默认转储Stream工厂
    /// </summary>
    public class DefaultDumpStreamFactory : IDumpStreamFactory
    {
        /// <summary>
        /// 初始化 <see cref="MemoryStream"/> 的容量
        /// </summary>
        public int InitialCapacity { get; }

        /// <summary>
        /// 默认转储Stream工厂
        /// </summary>
        /// <param name="initialCapacity"></param>
        public DefaultDumpStreamFactory(int initialCapacity)
        {
            InitialCapacity = initialCapacity;
        }

        /// <inheritdoc/>
        public MemoryStream Create() => new MemoryStream(InitialCapacity);
    }
}