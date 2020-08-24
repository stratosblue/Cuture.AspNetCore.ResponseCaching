using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests
{
    [TestClass]
    public class CacheByQueryTest : BaseRequestTest
    {
        protected override Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
        {
            return new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
                () => $"{BaseUrl}/WeatherForecast/get-q?page=1&pageSize=5".ToHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/get-q?page=1&pageSize=6".ToHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/get-q?page=2&pageSize=4".ToHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/get-q?page=2&pageSize=6".ToHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/get-q?page=3&pageSize=3".ToHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            };
        }
    }
}
