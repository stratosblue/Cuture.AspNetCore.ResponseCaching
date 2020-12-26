namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 可以转换为缓存Key
    /// </summary>
    public interface ICacheKeyable
    {
        #region Public 方法

        /// <summary>
        /// 作为缓存Key
        /// </summary>
        /// <returns></returns>
        string AsCacheKey();

        #endregion Public 方法
    }
}