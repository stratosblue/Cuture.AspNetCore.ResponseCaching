using Cuture.Http;
using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CustomCachingProcessInterceptorTest : RequestTestBase
{
    #region Public 方法

    [TestMethod]
    public async Task Should_Equals_With_Different_Request_Type()
    {
        var funcs = new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByPathWithCustomInterceptor/Get?page=1&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByPathWithCustomInterceptor/Get?page=1".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByPathWithCustomInterceptor/Get".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByPathWithCustomInterceptor/Get?page=1&pageSize=5".CreateHttpRequest().UsePost().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByPathWithCustomInterceptor/Get?page=1".CreateHttpRequest().UsePost().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByPathWithCustomInterceptor/Get".CreateHttpRequest().UsePost().TryGetAsObjectAsync<WeatherForecast[]>(),
        };
        await ExecuteAsync(funcs, true, false, 3, false);
    }

    #endregion Public 方法
}
