using Cuture.AspNetCore.ResponseCaching;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;
using ResponseCaching.Test.WebHost;
using StackExchange.Redis;

namespace ResponseCaching.Test.ResponseCaches;

[TestClass]
public class RedisResponseCacheTest : ResponseCacheTest
{
    #region Protected 字段

    protected ConnectionMultiplexer ConnectionMultiplexer = null!;

    protected GarnetServerDescriptor GarnetServerDescriptor = null!;

    #endregion Protected 字段

    #region Public 方法

    [TestCleanup]
    public override void Cleanup()
    {
        GarnetServerDescriptor?.Dispose();
        GarnetServerDescriptor = null!;
        ConnectionMultiplexer?.Dispose();
        ConnectionMultiplexer = null!;
    }

    #endregion Public 方法

    #region Protected 方法

    protected override async Task<IResponseCache> GetResponseCache()
    {
        GarnetServerDescriptor = GarnetServerUtil.Create();
        ConnectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(GarnetServerDescriptor.RedisConfigureString);
        return new RedisResponseCache(new RedisResponseCacheOptions() { ConnectionMultiplexer = ConnectionMultiplexer });
    }

    #endregion Protected 方法
}
