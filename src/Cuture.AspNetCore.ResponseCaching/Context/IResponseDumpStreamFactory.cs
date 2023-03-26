using System.IO;

namespace Cuture.AspNetCore.ResponseCaching;

/// <summary>
/// 创建用于转储响应的 <see cref="MemoryStream"/> 工厂
/// </summary>
public interface IResponseDumpStreamFactory
{
    #region Public 方法

    /// <summary>
    /// 创建转储<see cref="MemoryStream"/>
    /// </summary>
    /// <param name="initialCapacity"></param>
    /// <returns></returns>
    MemoryStream Create(int initialCapacity);

    #endregion Public 方法
}