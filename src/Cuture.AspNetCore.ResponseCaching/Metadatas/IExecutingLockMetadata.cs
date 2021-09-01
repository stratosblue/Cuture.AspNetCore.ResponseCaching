using Cuture.AspNetCore.ResponseCaching.Lockers;

using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseCaching.Metadatas
{
    /// <summary>
    /// <inheritdoc cref="IResponseCachingMetadata"/> - 缓存通行模式
    /// </summary>
    public interface IExecutingLockMetadata : IResponseCachingMetadata
    {
        #region Public 属性

        /// <summary>
        /// 使用的 <inheritdoc cref="IRequestExecutingLocker{TExecutingContext, TCachingData}"/> 名称
        /// </summary>
        string LockerName { get; }

        /// <summary>
        /// 缓存通行模式
        /// </summary>
        ExecutingLockMode LockMode { get; }

        #endregion Public 属性
    }
}