namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 转储Stream工厂提供器
    /// </summary>
    public interface IDumpStreamFactoryProvider
    {
        #region Public 方法

        /// <summary>
        /// 创建 <see cref="IDumpStreamFactory"/>
        /// </summary>
        /// <returns></returns>
        IDumpStreamFactory Create();

        #endregion Public 方法
    }
}