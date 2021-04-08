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

        /// <summary>
        /// 过期时间
        /// <para/>
        /// 基于 <see cref="DateTime.UtcNow"/> 的过期Unix时间戳
        /// <para/>
        /// 单位：毫秒
        /// </summary>
        public long Expire { get; }

        #endregion Public 属性

        #region Public 构造函数

        /// <summary>
        /// 响应缓存条目
        /// </summary>
        /// <param name="contentType">ContentType</param>
        /// <param name="body">内容</param>
        /// <param name="absoluteExpire">基于 <see cref="DateTime.UtcNow"/> 的绝对过期Unix时间戳</param>
        public ResponseCacheEntry(string contentType, ReadOnlyMemory<byte> body, long absoluteExpire)
        {
            ContentType = contentType;
            Body = body;
            Expire = absoluteExpire;
        }

        /// <summary>
        /// 响应缓存条目
        /// </summary>
        /// <param name="contentType">ContentType</param>
        /// <param name="body">内容</param>
        /// <param name="duration">有效时长</param>
        public ResponseCacheEntry(string contentType, ReadOnlyMemory<byte> body, int duration)
        {
            ContentType = contentType;
            Body = body;
            Expire = DateTimeOffset.UtcNow.AddSeconds(duration).ToUnixTimeMilliseconds();
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <summary>
        /// 获取绝对过期时间的 <see cref="DateTimeOffset"/>
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset GetAbsoluteExpirationDateTimeOffset() => DateTimeOffset.FromUnixTimeMilliseconds(Expire);

        /// <summary>
        /// 获取绝对过期时间的 <see cref="DateTimeOffset.UtcDateTime"/>
        /// </summary>
        /// <returns></returns>
        public DateTime GetAbsoluteExpirationUtcDateTime() => DateTimeOffset.FromUnixTimeMilliseconds(Expire).UtcDateTime;

        /// <summary>
        /// 现在是否已过期
        /// </summary>
        /// <returns></returns>
        public bool IsExpired() => DateTimeOffset.FromUnixTimeMilliseconds(Expire) < DateTimeOffset.UtcNow;

        #endregion Public 方法
    }
}