using System.IO;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 转储Stream工厂
    /// </summary>
    public interface IDumpStreamFactory
    {
        /// <summary>
        /// 创建转储Stream
        /// </summary>
        /// <returns></returns>
        MemoryStream Create();
    }
}