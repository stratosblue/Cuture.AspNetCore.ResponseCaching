using System;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 缓存依据的键没有找到
    /// </summary>
    public class CacheVaryKeyNotFindException : ArgumentNullException
    {
        #region Public 构造函数

        /// <inheritdoc/>
        public CacheVaryKeyNotFindException(string keyName) : base($"Cache VaryKey Not Find: {keyName}")
        {
        }

        /// <inheritdoc/>
        public CacheVaryKeyNotFindException()
        {
        }

        /// <inheritdoc/>
        public CacheVaryKeyNotFindException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion Public 构造函数
    }
}