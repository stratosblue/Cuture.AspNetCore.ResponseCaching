using System;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using StackExchange.Redis;

namespace ResponseCaching.Test.ResponseCaches
{
    [TestClass]
    public class RedisResponseCacheTest : ResponseCacheTest
    {
        private ConnectionMultiplexer _connectionMultiplexer;

        protected override async Task<IResponseCache> GetResponseCache()
        {
            var configuration = Environment.GetEnvironmentVariable("ResponseCache_Test_Redis");
            _connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configuration);
            return new RedisResponseCache(new RedisResponseCacheOptions() { ConnectionMultiplexer = _connectionMultiplexer });
        }

        [TestCleanup]
        public override void Cleanup()
        {
            _connectionMultiplexer.Dispose();
            _connectionMultiplexer = null;
        }
    }
}