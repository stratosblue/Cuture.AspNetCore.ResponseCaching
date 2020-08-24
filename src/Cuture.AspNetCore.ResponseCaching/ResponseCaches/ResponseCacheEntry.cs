using System;

namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches
{
    /// <summary>
    /// 响应缓存条目
    /// </summary>
    public class ResponseCacheEntry
    {
        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// 内容
        /// </summary>
        public ReadOnlyMemory<byte> Body { get; }

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
    }
}