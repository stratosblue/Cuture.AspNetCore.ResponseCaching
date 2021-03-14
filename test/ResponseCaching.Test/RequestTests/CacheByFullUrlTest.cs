using System;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests
{
    [TestClass]
    public class CacheByFullUrlTest : BaseRequestTest
    {
        protected override int ReRequestTimes => 4;

        protected override Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
        {
            return new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
                () => $"{BaseUrl}/WeatherForecast/url-cache?page=1&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/url-cache?page=2&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/url-cache?page=3&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/url-cache?page=1&pageSize=5&_t=1".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/url-cache?page=1&pageSize=5&_t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/url-cache?page=1&pageSize=5&_t=1&_t1=0".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            };
        }
    }
}
