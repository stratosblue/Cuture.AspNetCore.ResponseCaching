using System;
using System.Linq;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.Base;
using ResponseCaching.Test.WebHost.Dtos;
using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.Interceptors
{
    public class CacheHitStampHeaderTestBase : WebServerHostedTestBase
    {
        protected const string HeaderKey = "cached";
        protected const string HeaderValue = "1";

        protected Task<TextHttpOperationResult<WeatherForecast[]>>[] GetRequestTasks()
        {
            return new Task<TextHttpOperationResult<WeatherForecast[]>>[] {
                $"{BaseUrl}/WeatherForecast/url-cache?page=1&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                $"{BaseUrl}/WeatherForecast/url-cache?page=2&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                $"{BaseUrl}/WeatherForecast/url-cache?page=3&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                $"{BaseUrl}/WeatherForecast/url-cache?page=1&pageSize=5&_t=1".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                $"{BaseUrl}/WeatherForecast/url-cache?page=1&pageSize=5&_t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                $"{BaseUrl}/WeatherForecast/url-cache?page=1&pageSize=5&_t=1&_t1=0".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
                $"{BaseUrl}/WeatherForecast/get-m".CreateHttpRequest().UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 1,PageSize = 5 }).TryGetAsObjectAsync<WeatherForecast[]>(),
                $"{BaseUrl}/WeatherForecast/get-m".CreateHttpRequest().UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 1,PageSize = 6 }).TryGetAsObjectAsync<WeatherForecast[]>(),
                $"{BaseUrl}/WeatherForecast/get-m".CreateHttpRequest().UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 2,PageSize = 4 }).TryGetAsObjectAsync<WeatherForecast[]>(),
                $"{BaseUrl}/WeatherForecast/get-m".CreateHttpRequest().UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 2,PageSize = 6 }).TryGetAsObjectAsync<WeatherForecast[]>(),
                $"{BaseUrl}/WeatherForecast/get-m".CreateHttpRequest().UsePost().WithJsonContent(new PageResultRequestDto(){ Page = 3,PageSize = 3 }).TryGetAsObjectAsync<WeatherForecast[]>(),
            };
        }

        protected async Task CheckRequestsAsync(Task<TextHttpOperationResult<WeatherForecast[]>>[] tasks, Action<TextHttpOperationResult<WeatherForecast[]>> action)
        {
            foreach (var task in tasks)
            {
                var httpResult = await task;
                action(httpResult);
            }
        }

        protected void CheckNoCacheHeader(TextHttpOperationResult<WeatherForecast[]> httpResult)
        {
            var headerExist = httpResult.ResponseMessage.Headers.TryGetValues(HeaderKey, out _);
            Assert.IsFalse(headerExist);
        }

        protected void CheckHasCacheHeader(TextHttpOperationResult<WeatherForecast[]> httpResult)
        {
            var headerExist = httpResult.ResponseMessage.Headers.TryGetValues(HeaderKey, out var value);
            Assert.IsTrue(headerExist);
            Assert.AreEqual(HeaderValue, value.First());
        }
    }

    [TestClass]
    public class CacheHitStampHeaderTest : CacheHitStampHeaderTestBase
    {
        protected override Task ConfigureWebHost(IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services =>
            {
                services.AddCaching().UseCacheHitStampHeader(HeaderKey, HeaderValue);
            });
            return Task.CompletedTask;
        }

        [TestMethod]
        public async Task Check()
        {
            await CheckRequestsAsync(GetRequestTasks(), httpResult => CheckNoCacheHeader(httpResult));
            await Task.Delay(100);
            for (int i = 0; i < 5; i++)
            {
                await CheckRequestsAsync(GetRequestTasks(), httpResult => CheckHasCacheHeader(httpResult));
            }
        }
    }

    [TestClass]
    public class NoneCacheHitStampHeaderTest : CacheHitStampHeaderTestBase
    {
        [TestMethod]
        public async Task Check()
        {
            for (int i = 0; i < 5; i++)
            {
                await CheckRequestsAsync(GetRequestTasks(), httpResult => CheckNoCacheHeader(httpResult));
            }
        }
    }
}