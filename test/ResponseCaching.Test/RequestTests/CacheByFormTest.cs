using System;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByFormTest : RequestTestBase
{
    #region Public 方法

    [TestMethod]
    public async Task Should_Different_With_Different_FormData()
    {
        var funcs = new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByForm/Post".CreateHttpRequest().UsePost().WithFormContent("page=1&pageSize=5").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByForm/Post".CreateHttpRequest().UsePost().WithFormContent("page=1&pageSize=6").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByForm/Post".CreateHttpRequest().UsePost().WithFormContent("page=2&pageSize=4").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByForm/Post".CreateHttpRequest().UsePost().WithFormContent("page=2&pageSize=6").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByForm/Post".CreateHttpRequest().UsePost().WithFormContent("page=3&pageSize=3").TryGetAsObjectAsync<WeatherForecast[]>(),
        };
        await ExecuteAsync(funcs, true, false, 3);
    }

    #endregion Public 方法
}
