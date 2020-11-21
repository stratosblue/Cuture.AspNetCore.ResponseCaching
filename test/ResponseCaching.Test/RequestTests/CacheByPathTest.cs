using System;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests
{
    [TestClass]
    public class CacheByPathTest : BaseRequestTest
    {
        protected override bool CheckEachOtherRequest => false;

        protected override async Task BeforeRunning()
        {
            //预请求，保证两种请求方式数据一致
            var funcs = GetAllRequestFuncs();
            await funcs[new Random().Next(funcs.Length)].Invoke();
        }

        protected override Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
        {
            return new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
                () => $"{BaseUrl}/WeatherForecast/path-cache?page=1&pageSize=5".ToHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/path-cache?page=1".ToHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/path-cache".ToHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/path-cache?page=1&pageSize=5".ToHttpRequest().UsePost().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/path-cache?page=1".ToHttpRequest().UsePost().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/path-cache".ToHttpRequest().UsePost().TryGetAsObjectAsync<WeatherForecast[]>(),
            };
        }
    }
}
