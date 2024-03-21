using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByQueryTest : RequestTestBase
{
    #region Public 方法

    [TestMethod]
    public async Task Should_Different_With_Different_Query()
    {
        var funcs = new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByQuery/Get?page=1&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQuery/Get?page=1&pageSize=6".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQuery/Get?page=2&pageSize=4".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQuery/Get?page=2&pageSize=6".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQuery/Get?page=3&pageSize=3".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
        };
        await ExecuteAsync(funcs, true, false, 3);
    }

    #endregion Public 方法
}
