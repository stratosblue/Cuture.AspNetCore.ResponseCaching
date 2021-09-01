namespace Cuture.AspNetCore.ResponseCaching.Metadatas
{
    /// <summary>
    /// <inheritdoc cref="IResponseCachingMetadata"/> - 响应Dump的初始容量
    /// </summary>
    public interface IResponseDumpCapacityMetadata : IResponseCachingMetadata
    {
        #region Public 属性

        /// <summary>
        /// 响应Dump的初始容量
        /// </summary>
        int Capacity { get; }

        #endregion Public 属性
    }
}