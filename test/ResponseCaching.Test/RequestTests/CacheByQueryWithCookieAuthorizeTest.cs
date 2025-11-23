using Cuture.Http;
using Cuture.Http.Util;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByQueryWithCookieAuthorizeTest : RequestTestBase
{
    #region Private 字段

    private string[] _cookies = null;

    #endregion Private 字段

    #region Public 方法

    [TestInitialize]
    public override async Task InitAsync()
    {
        await base.InitAsync();

        var cookies = new List<string>();
        for (int i = 1; i < 6; i++)
        {
            var result = await $"{BaseUrl}/login/cookie?uid=testuser{i}".CreateHttpRequest().TryGetAsStringAsync();
            var cookie = CookieUtility.Clean(result.ResponseMessage?.GetCookie());

            cookies.Add(cookie);
        }
        _cookies = cookies.ToArray();
    }

    [TestMethod]
    public async Task Should_Different_With_Different_Cookie()
    {
        var funcs = new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByQueryWithAuthorize/Get?page=1&pageSize=5".CreateHttpRequest().UseCookie(_cookies[0]).TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQueryWithAuthorize/Get?page=1&pageSize=6".CreateHttpRequest().UseCookie(_cookies[1]).TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQueryWithAuthorize/Get?page=2&pageSize=4".CreateHttpRequest().UseCookie(_cookies[2]).TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQueryWithAuthorize/Get?page=2&pageSize=6".CreateHttpRequest().UseCookie(_cookies[3]).TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQueryWithAuthorize/Get?page=3&pageSize=3".CreateHttpRequest().UseCookie(_cookies[4]).TryGetAsObjectAsync<WeatherForecast[]>(),
        };
        await ExecuteAsync(funcs, true, false, 3);
    }

    #endregion Public 方法
}
