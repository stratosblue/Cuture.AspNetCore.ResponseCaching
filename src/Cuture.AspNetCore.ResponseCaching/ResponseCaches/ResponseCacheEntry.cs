using System;

namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches
{
    /// <summary>
    /// 响应缓存条目
    /// </summary>
    public class ResponseCacheEntry
    {
        #region Public 属性

        /// <summary>
        /// 内容
        /// </summary>
        public ReadOnlyMemory<byte> Body { get; }

        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; }

        #endregion Public 属性

        #region Public 构造函数

        /// <summary>
        /// 响应缓存条目
        /// </summary>
        /// <param name="contentType">ContentType</param>
        /// <param name="body">内容</param>
        public ResponseCacheEntry(string contentType, ReadOnlyMemory<byte> body)
        {
            ContentType = contentType;
            Body = body;
        }

        #endregion Public 构造函数
    }
}