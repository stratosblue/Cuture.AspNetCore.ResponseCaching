using System;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

/// <summary>
/// 没有缓存的接口请求测试
/// </summary>
[TestClass]
public class NoneCachingTest : RequestTestBase
{
    #region Public 方法

    [TestMethod]
    public async Task Should_Different_With_ReRequest()
    {
        var funcs = new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/WeatherForecast/get?page=1&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/WeatherForecast/get?page=1&pageSize=6".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/WeatherForecast/get?page=2&pageSize=4".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/WeatherForecast/get?page=2&pageSize=6".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/WeatherForecast/get?page=3&pageSize=3".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
        };
        await ExecuteAsync(funcs, true, false, 3, false);
    }

    #endregion Public 方法
}
