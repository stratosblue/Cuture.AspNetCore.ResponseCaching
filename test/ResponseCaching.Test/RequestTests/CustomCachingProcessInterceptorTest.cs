using System;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Dtos;
using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests
{
    [TestClass]
    public class CustomCachingProcessInterceptorTest : BaseRequestTest
    {
        protected override Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
        {
            return new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
                () => $"{BaseUrl}/CacheByPathWithCustomInterceptor/Get?page=1&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/CacheByPathWithCustomInterceptor/Get?page=1".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/CacheByPathWithCustomInterceptor/Get".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/CacheByPathWithCustomInterceptor/Get?page=1&pageSize=5".CreateHttpRequest().UsePost().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/CacheByPathWithCustomInterceptor/Get?page=1".CreateHttpRequest().UsePost().TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/CacheByPathWithCustomInterceptor/Get".CreateHttpRequest().UsePost().TryGetAsObjectAsync<WeatherForecast[]>(),
            };
        }

        [TestMethod]
        public override async Task ExecuteAsync()
        {
            var funcs = GetAllRequestFuncs();

            var data = await InternalRunAsync(funcs);

            Assert.IsTrue(data.Length > 0);

            for (int i = 0; i < data.Length - 1; i++)
            {
                for (int j = i + 1; j < data.Length; j++)
                {
                    AreNotEqual(data[i], data[j]);
                }
            }
        }
    }
}
