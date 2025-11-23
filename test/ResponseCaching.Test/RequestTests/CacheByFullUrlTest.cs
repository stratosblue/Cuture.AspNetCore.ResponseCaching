using Cuture.Http;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByFullUrlTest : RequestTestBase
{
    #region Private 字段

    private readonly long _t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    #endregion Private 字段

    #region Public 方法

    [TestMethod]
    public async Task Should_Different_With_Different_Url()
    {
        var funcs = new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByFullUrl/Get?page=1&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullUrl/Get?page=2&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullUrl/Get?page=3&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullUrl/Get?page=1&pageSize=5&_t=1".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullUrl/Get?page=1&pageSize=5&_t={_t}".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullUrl/Get?page=1&pageSize=5&_t=1&_t1=0".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
        };

        await ExecuteAsync(funcs, true, false, 4);
    }

    #endregion Public 方法
}
