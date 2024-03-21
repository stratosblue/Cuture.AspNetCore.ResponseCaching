using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByFullQueryTest : RequestTestBase
{
    #region Public 方法

    [TestMethod]
    public async Task Should_Different_With_Different_QueryKeys()
    {
        var funcs = new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByFullQuery/Get?page=1&pageSize=3".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?page=1&pageSize=3&t".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?page=1&pageSize=3&=t".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?page=1&pageSize=3&=0".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?page=1&pageSize=3&t=0".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
        };
        await ExecuteAsync(funcs, true, false, 3);
    }

    [TestMethod]
    public async Task Should_Equals_With_Different_QueryKey_Order()
    {
        var funcs = new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByFullQuery/Get?page=1&pageSize=3&t=1&t3=1&t2=1&t800".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?t=1&t3=1&t2=1&t800&page=1&pageSize=3".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?t=1&page=1&t3=1&t800&pageSize=3&t2=1".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?t3=1&page=1&t=1&t2=1&t800&pageSize=3".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?t2=1&page=1&pageSize=3&t=1&t3=1&t800".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
        };
        await ExecuteAsync(funcs, true, true, 3);
    }

    #endregion Public 方法
}
