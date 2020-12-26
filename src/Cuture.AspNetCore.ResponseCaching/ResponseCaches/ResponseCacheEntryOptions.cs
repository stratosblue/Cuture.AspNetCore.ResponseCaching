namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches
{
    /// <summary>
    /// 响应缓存选项
    /// </summary>
    public class ResponseCacheEntryOptions
    {
        #region Public 属性

        /// <summary>
        /// 缓存时长（秒）
        /// </summary>
        public int Duration { get; set; }

        #endregion Public 属性
    }
}