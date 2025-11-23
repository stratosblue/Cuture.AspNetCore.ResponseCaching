using Cuture.Http;
using Cuture.Http.Util;

using ResponseCaching.Test.WebHost.Dtos;
using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByAllMixedTest : RequestTestBase
{
    #region Private 字段

    private const int CookieCount = 5;

    private string[] _cookies = null!;

    #endregion Private 字段

    #region Public 方法

    [TestInitialize]
    public override async Task InitAsync()
    {
        await base.InitAsync();

        var cookies = new List<string>();
        for (int i = 1; i < CookieCount; i++)
        {
            var result = await $"{BaseUrl}/login/cookie?uid=testuser{i}".CreateHttpRequest().TryGetAsStringAsync();
            var cookie = CookieUtility.Clean(result.ResponseMessage.GetCookie());

            cookies.Add(cookie);
        }
        _cookies = cookies.ToArray();
    }

    [TestMethod]
    public async Task Should_Different_With_CacheByAllMixed()
    {
        await ExecuteAsync(GetAllRequestFuncs(), true, false, 3);
    }

    [TestMethod]
    public async Task Should_Different_With_CacheByAllMixed_PostWithPath()
    {
        await ExecuteAsync(GetAllRequestFuncs2(), true, false, 3);
    }

    #endregion Public 方法

    #region Private 方法

    private Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
    {
        var result = new List<Func<Task<TextHttpOperationResult<WeatherForecast[]>>>>();

        foreach (var cookie in _cookies)
        {
            for (int page_q = 1; page_q < 10; page_q += 3)
            {
                for (int pageSize_q = 7; pageSize_q < 15; pageSize_q += 4)
                {
                    var url = $"{BaseUrl}/CacheByAllMixed/Post?page={page_q}&pageSize={pageSize_q}";
                    for (int page_h = 1; page_h < 10; page_h += 3)
                    {
                        for (int pageSize_h = 7; pageSize_h < 15; pageSize_h += 4)
                        {
                            for (int page_m = 1; page_m < 10; page_m += 3)
                            {
                                for (int pageSize_m = 7; pageSize_m < 15; pageSize_m += 4)
                                {
                                    var t_page_m = page_m;
                                    var t_pageSize_m = pageSize_m;
                                    var t_page_h = page_h;
                                    var t_pageSize_h = pageSize_h;
                                    result.Add(() => url.CreateHttpRequest()
                                                        .UsePost()
                                                        .WithJsonContent(new PageResultRequestDto() { Page = t_page_m, PageSize = t_pageSize_m })
                                                        .AddHeader("page", t_page_h.ToString())
                                                        .AddHeader("pageSize", t_pageSize_h.ToString())
                                                        .UseCookie(cookie)
                                                        .TryGetAsObjectAsync<WeatherForecast[]>());
                                }
                            }
                        }
                    }
                }
            }
        }

        return result.ToArray();
    }

    private Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs2()
    {
        var result = new List<Func<Task<TextHttpOperationResult<WeatherForecast[]>>>>();

        foreach (var cookie in _cookies)
        {
            for (int page_q = 1; page_q < 10; page_q += 3)
            {
                for (int pageSize_q = 7; pageSize_q < 15; pageSize_q += 4)
                {
                    for (int Value1 = 0; Value1 < 2; Value1 += 1)
                    {
                        for (int Value2 = 0; Value2 < 2; Value2 += 1)
                        {
                            var url = $"{BaseUrl}/CacheByAllMixed/PostWithPath/{Value1}/{Value2}?page={page_q}&pageSize={pageSize_q}";
                            for (int page_h = 1; page_h < 10; page_h += 3)
                            {
                                for (int pageSize_h = 7; pageSize_h < 15; pageSize_h += 4)
                                {
                                    for (int page_m = 1; page_m < 10; page_m += 3)
                                    {
                                        for (int pageSize_m = 7; pageSize_m < 15; pageSize_m += 4)
                                        {
                                            var t_page_m = page_m;
                                            var t_pageSize_m = pageSize_m;
                                            var t_page_h = page_h;
                                            var t_pageSize_h = pageSize_h;
                                            result.Add(() => url.CreateHttpRequest()
                                                                .UsePost()
                                                                .WithJsonContent(new PageResultRequestDto() { Page = t_page_m, PageSize = t_pageSize_m })
                                                                .AddHeader("page", t_page_h.ToString())
                                                                .AddHeader("pageSize", t_pageSize_h.ToString())
                                                                .UseCookie(cookie)
                                                                .TryGetAsObjectAsync<WeatherForecast[]>());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return result.ToArray();
    }

    #endregion Private 方法
}
