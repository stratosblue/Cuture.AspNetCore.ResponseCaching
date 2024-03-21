using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByPathTest : RequestTestBase
{
    #region Public 方法

    [TestMethod]
    public async Task Should_Equals_With_Multi_Request()
    {
        var funcs = new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByPath/Get?page=1&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByPath/Get?page=1".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByPath/Get".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByPath/Post?page=1&pageSize=5".CreateHttpRequest().UsePost().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByPath/Post?page=1".CreateHttpRequest().UsePost().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByPath/Post".CreateHttpRequest().UsePost().TryGetAsObjectAsync<WeatherForecast[]>(),
        };

        await ExecuteAsync(funcs, false, false, 3);
    }

    [TestMethod]
    public async Task Should_Different_With_Different_Path()
    {
        var funcs = new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByPath/RelativeRoute1/R1/1".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByPath/RelativeRoute1/R1/2".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByPath/RelativeRoute1/R1/3".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/R1/1".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/R1/2".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/R1/3".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
        };

        await ExecuteAsync(funcs, true, false, 3);
    }

    #endregion Public 方法
}
