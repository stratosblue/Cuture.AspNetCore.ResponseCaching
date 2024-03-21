using Cuture.AspNetCore.ResponseCaching;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost;

using StackExchange.Redis;

namespace ResponseCaching.Test.ResponseCaches;

[TestClass]
public class RedisResponseCacheTest : ResponseCacheTest
{
    private ConnectionMultiplexer _connectionMultiplexer;

    protected override async Task<IResponseCache> GetResponseCache()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true)
                                                      .AddJsonFile("appsettings.Development.json", true)
                                                      .AddEnvironmentVariables()
                                                      .AddUserSecrets<TestWebHost>()
                                                      .Build()
                                                      .GetValue<string>("ResponseCache_Test_Redis");
        if (string.IsNullOrWhiteSpace(configuration))
        {
            throw new ArgumentException("Must set ‘ResponseCache_Test_Redis’ first.");
        }
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
