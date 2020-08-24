using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;

namespace Cuture.AspNetCore.ResponseCaching.ResponseCaches
{
    /// <summary>
    /// 基于 <see cref="StackExchange.Redis"/> 的响应缓存
    /// </summary>
    public class RedisResponseCache : IDistributedResponseCache
    {
        /// <summary>
        /// ContenType Hash字段名称
        /// </summary>
        public const string ContenTypeFieldName = "ContenType";

        /// <summary>
        /// BodyFieldName Hash字段名称
        /// </summary>
        public const string BodyFieldName = "Body";

        /// <summary>
        /// Expire Hash字段名称
        /// </summary>
        public const string ExpireFieldName = "Expire";

        private static readonly RedisValue[] _fieldNames = new RedisValue[] { ContenTypeFieldName, BodyFieldName, ExpireFieldName };

        private readonly IDatabase _database;

        private readonly RedisValue _bodyFieldName = BodyFieldName;
        private readonly RedisValue _expireFieldName = ExpireFieldName;
        private readonly RedisValue _contenTypeFieldName = ContenTypeFieldName;

        /// <summary>
        /// 基于 <see cref="StackExchange.Redis"/> 的响应缓存
        /// </summary>
        /// <param name="optionAccessor">缓存选项</param>
        public RedisResponseCache(IOptions<RedisResponseCacheOptions> optionAccessor)
        {
            var options = optionAccessor.Value;
            _database = options.ConnectionMultiplexer.GetDatabase();
            var prefix = options.CacheKeyPrefix;
            if (!string.IsNullOrEmpty(prefix))
            {
                _database = _database.WithKeyPrefix(prefix);
            }
        }

        /// <inheritdoc/>
        public async Task<ResponseCacheEntry> GetAsync(string key)
        {
            var redisValues = await _database.HashGetAsync(key, _fieldNames);
            if (redisValues[0].IsNull || redisValues[1].IsNull || redisValues[2].IsNull)
            {
                return null;
            }
            if ((long)redisValues[2] < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                return null;
            }
            return new ResponseCacheEntry(redisValues[0], redisValues[1]);
        }

        /// <inheritdoc/>
        public async Task SetAsync(string key, ResponseCacheEntry entry, int duration)
        {
            RedisKey redisKey = key;
            await _database.HashSetAsync(redisKey, new[] {
                new HashEntry(_contenTypeFieldName, entry.ContentType),
                new HashEntry(_bodyFieldName, entry.Body),
                new HashEntry(_expireFieldName, DateTimeOffset.UtcNow.AddSeconds(duration).ToUnixTimeSeconds()),
            });
            _ = _database.KeyExpireAsync(redisKey, TimeSpan.FromSeconds(duration));
        }
    }
}