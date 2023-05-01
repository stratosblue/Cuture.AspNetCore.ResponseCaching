using System;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByHeaderTest : RequestTestBase
{
    #region Public 方法

    [TestMethod]
    public async Task Should_Different_With_Different_Headers()
    {
        var funcs = new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByHeader/Get".CreateHttpRequest().AddHeader("page","1").AddHeader("pageSize","5").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByHeader/Get".CreateHttpRequest().AddHeader("page","1").AddHeader("pageSize","6").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByHeader/Get".CreateHttpRequest().AddHeader("page","2").AddHeader("pageSize","4").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByHeader/Get".CreateHttpRequest().AddHeader("page","2").AddHeader("pageSize","6").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByHeader/Get".CreateHttpRequest().AddHeader("page","3").AddHeader("pageSize","3").TryGetAsObjectAsync<WeatherForecast[]>(),
        };

        await ExecuteAsync(funcs, true, false, 3);
    }

    #endregion Public 方法
}
