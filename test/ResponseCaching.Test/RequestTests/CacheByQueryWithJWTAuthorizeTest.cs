using Cuture.Http;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByQueryWithJWTAuthorizeTest : RequestTestBase
{
    #region Private 字段

    private string[] _jwts = null;

    #endregion Private 字段

    #region Public 方法

    [TestInitialize]
    public override async Task InitAsync()
    {
        await base.InitAsync();

        var jwts = new List<string>();
        for (int i = 1; i < 6; i++)
        {
            var token = await $"{BaseUrl}/login/jwt?uid=testuser{i}".CreateHttpRequest().GetAsStringAsync();

            jwts.Add(token);
        }
        _jwts = jwts.ToArray();
    }

    [TestMethod]
    public async Task Should_Different_With_Different_Authorization()
    {
        var funcs = new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByQueryWithAuthorize/Get?page=1&pageSize=5".CreateHttpRequest().AddHeader("Authorization", $"Bearer {_jwts[0]}").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQueryWithAuthorize/Get?page=1&pageSize=6".CreateHttpRequest().AddHeader("Authorization", $"Bearer {_jwts[1]}").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQueryWithAuthorize/Get?page=2&pageSize=4".CreateHttpRequest().AddHeader("Authorization", $"Bearer {_jwts[2]}").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQueryWithAuthorize/Get?page=2&pageSize=6".CreateHttpRequest().AddHeader("Authorization", $"Bearer {_jwts[3]}").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQueryWithAuthorize/Get?page=3&pageSize=3".CreateHttpRequest().AddHeader("Authorization", $"Bearer {_jwts[4]}").TryGetAsObjectAsync<WeatherForecast[]>(),
        };
        await ExecuteAsync(funcs, true, false, 3);
    }

    #endregion Public 方法
}
