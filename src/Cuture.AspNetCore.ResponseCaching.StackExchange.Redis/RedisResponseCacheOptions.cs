using Microsoft.Extensions.Options;

using StackExchange.Redis;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// redis响应缓存选项
    /// </summary>
    public class RedisResponseCacheOptions : IOptions<RedisResponseCacheOptions>
    {
        #region Public 属性

        /// <summary>
        /// 缓存Key前缀
        /// </summary>
        public string? CacheKeyPrefix { get; set; }

        /// <summary>
        /// 连接配置
        /// </summary>
        public string? Configuration { get; set; }

        /// <summary>
        ///
        /// </summary>
        public IConnectionMultiplexer? ConnectionMultiplexer { get; set; }

        /// <summary>
        ///
        /// </summary>
        public RedisResponseCacheOptions Value => this;

        #endregion Public 属性
    }
}