using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Cuture.Http;
using Cuture.Http.Util;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests
{
    [TestClass]
    public class CacheByQueryWithCookieAuthorizeTest : BaseRequestTest
    {
        private string[] _cookies = null;

        [TestInitialize]
        public override async Task InitAsync()
        {
            await base.InitAsync();

            var cookies = new List<string>();
            for (int i = 1; i < 6; i++)
            {
                var result = await $"{BaseUrl}/login/cookie?uid=testuser{i}".CreateHttpRequest().TryGetAsStringAsync();
                var cookie = CookieUtility.Clean(result.ResponseMessage.GetCookie());

                cookies.Add(cookie);
            }
            _cookies = cookies.ToArray();
        }

        protected override Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
        {
            return new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
                () => $"{BaseUrl}/WeatherForecast/get-at-q?page=1&pageSize=5".CreateHttpRequest().UseCookie(_cookies[0]).TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/get-at-q?page=1&pageSize=6".CreateHttpRequest().UseCookie(_cookies[1]).TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/get-at-q?page=2&pageSize=4".CreateHttpRequest().UseCookie(_cookies[2]).TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/get-at-q?page=2&pageSize=6".CreateHttpRequest().UseCookie(_cookies[3]).TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/get-at-q?page=3&pageSize=3".CreateHttpRequest().UseCookie(_cookies[4]).TryGetAsObjectAsync<WeatherForecast[]>(),
            };
        }
    }
}