using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Cuture.Http;
using Cuture.Http.Util;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Dtos;
using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests
{
    [TestClass]
    public class CacheByAllMixedTest : BaseRequestTest
    {
        private const int CookieCount = 5;
        private string[] _cookies = null;

        [TestInitialize]
        public override async Task InitAsync()
        {
            await base.InitAsync();

            var cookies = new List<string>();
            for (int i = 1; i < CookieCount; i++)
            {
                var result = await $"{BaseUrl}/login/cookie?uid=testuser{i}".ToHttpRequest().TryGetAsStringAsync();
                var cookie = CookieUtility.Clean(result.ResponseMessage.GetCookie());

                cookies.Add(cookie);
            }
            _cookies = cookies.ToArray();
        }

        protected override Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
        {
            var result = new List<Func<Task<TextHttpOperationResult<WeatherForecast[]>>>>();

            foreach (var cookie in _cookies)
            {
                for (int page_q = 1; page_q < 10; page_q += 3)
                {
                    for (int pageSize_q = 7; pageSize_q < 15; pageSize_q += 4)
                    {
                        var url = $"{BaseUrl}/WeatherForecast/get-all-mixed?page={page_q}&pageSize={pageSize_q}";
                        for (int page_h = 1; page_h < 10; page_h += 3)
                        {
                            for (int pageSize_h = 7; pageSize_h < 15; pageSize_h += 4)
                            {
                                for (int page_m = 1; page_m < 10; page_m += 3)
                                {
                                    for (int pageSize_m = 7; pageSize_m < 15; pageSize_m += 4)
                                    {
                                        result.Add(() => url.ToHttpRequest()
                                                            .UsePost()
                                                            .WithJsonContent(new PageResultRequestDto() { Page = page_m, PageSize = pageSize_m })
                                                            .AddHeader("page", page_h.ToString())
                                                            .AddHeader("pageSize", pageSize_h.ToString())
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
    }
}