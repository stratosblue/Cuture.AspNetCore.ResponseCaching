namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 可以转换为缓存Key
    /// </summary>
    public interface ICacheKeyable
    {
        /// <summary>
        /// 作为缓存Key
        /// </summary>
        /// <returns></returns>
        string AsCacheKey();
    }
}