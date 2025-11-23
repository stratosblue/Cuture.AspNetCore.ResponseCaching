using Cuture.Http;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.Base;
using ResponseCaching.Test.WebHost.Dtos;

namespace ResponseCaching.Test.Interceptors;

[TestClass]
public class CacheKeyAccessorTest : WebServerHostedTestBase
{
    #region Private 字段

    private readonly long _t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    #endregion Private 字段

    #region Public 方法

    [TestMethod]
    public async Task CheckResponseCacheKey()
    {
        var tasks = Enumerable.Range(0, 10).Select(_ => InternalCheckResponseCacheKey()).ToArray();

        await Task.WhenAll(tasks);
    }

    private async Task InternalCheckResponseCacheKey()
    {
        var requests = GetRequests();
        var exectedCacheKeys = GetExpectedCacheKeys().Select(m => m.ToLowerInvariant()).ToArray();

        Assert.HasCount(requests.Length, exectedCacheKeys);

        var requestTasks = requests.Select(m => m.GetAsStringAsync()).ToArray();

        await Task.WhenAll(requestTasks);

        var responseCacheKeys = requestTasks.Select(m => m.Result).ToArray();

        CollectionAssert.AreEqual(exectedCacheKeys, responseCacheKeys);
    }

    #endregion Public 方法

    #region Protected 方法

    protected override Task ConfigureWebHost(IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(services => services.AddCaching().EnableCacheKeyAccessor());
        return base.ConfigureWebHost(hostBuilder);
    }

    protected string[] GetExpectedCacheKeys()
    {
        return [
            $"get:/CacheByFullUrlKeyAccess/Get:page=1&pageSize=5",
            $"get:/CacheByFullUrlKeyAccess/Get:page=2&pageSize=5",
            $"get:/CacheByFullUrlKeyAccess/Get:page=3&pageSize=5",
            $"get:/CacheByFullUrlKeyAccess/Get:_t=1&page=1&pageSize=5",
            $"get:/CacheByFullUrlKeyAccess/Get:_t={_t}&page=1&pageSize=5",
            $"get:/CacheByFullUrlKeyAccess/Get:_t=1&_t1=0&page=1&pageSize=5",
            $"post:/CacheByModelKeyAccess/post:input=1_5",
            $"post:/CacheByModelKeyAccess/Post:input=1_6",
            $"post:/CacheByModelKeyAccess/Post:input=2_4",
            $"post:/CacheByModelKeyAccess/Post:input=2_6",
            $"post:/CacheByModelKeyAccess/Post:input=3_3",
        ];
    }

    protected IHttpRequest[] GetRequests()
    {
        return [
            $"{BaseUrl}/CacheByFullUrlKeyAccess/Get?page=1&pageSize=5".CreateHttpRequest(true),
            $"{BaseUrl}/CacheByFullUrlKeyAccess/Get?page=2&pageSize=5".CreateHttpRequest(true),
            $"{BaseUrl}/CacheByFullUrlKeyAccess/Get?page=3&pageSize=5".CreateHttpRequest(true),
            $"{BaseUrl}/CacheByFullUrlKeyAccess/Get?page=1&pageSize=5&_t=1".CreateHttpRequest(true),
            $"{BaseUrl}/CacheByFullUrlKeyAccess/Get?page=1&pageSize=5&_t={_t}".CreateHttpRequest(true),
            $"{BaseUrl}/CacheByFullUrlKeyAccess/Get?page=1&pageSize=5&_t=1&_t1=0".CreateHttpRequest(true),
            $"{BaseUrl}/CacheByModelKeyAccess/Post".CreateHttpRequest(true).UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 1,PageSize = 5 }),
            $"{BaseUrl}/CacheByModelKeyAccess/Post".CreateHttpRequest(true).UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 1,PageSize = 6 }),
            $"{BaseUrl}/CacheByModelKeyAccess/Post".CreateHttpRequest(true).UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 2,PageSize = 4 }),
            $"{BaseUrl}/CacheByModelKeyAccess/Post".CreateHttpRequest(true).UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 2,PageSize = 6 }),
            $"{BaseUrl}/CacheByModelKeyAccess/Post".CreateHttpRequest(true).UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 3,PageSize = 3 }),
        ];
    }

    #endregion Protected 方法
}
