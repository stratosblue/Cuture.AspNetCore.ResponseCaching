using System.IO;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 转储Stream工厂
    /// </summary>
    public interface IDumpStreamFactory
    {
        #region Public 方法

        /// <summary>
        /// 创建转储Stream
        /// </summary>
        /// <returns></returns>
        MemoryStream Create();

        #endregion Public 方法
    }
}