using System;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests
{
    [TestClass]
    public class CacheByFormTest : BaseRequestTest
    {
        protected override Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
        {
            return new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
                () => $"{BaseUrl}/WeatherForecast/get-f".ToHttpRequest().UsePost().WithFormContent("page=1&pageSize=5").TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/get-f".ToHttpRequest().UsePost().WithFormContent("page=1&pageSize=6").TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/get-f".ToHttpRequest().UsePost().WithFormContent("page=2&pageSize=4").TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/get-f".ToHttpRequest().UsePost().WithFormContent("page=2&pageSize=6").TryGetAsObjectAsync<WeatherForecast[]>(),
                () => $"{BaseUrl}/WeatherForecast/get-f".ToHttpRequest().UsePost().WithFormContent("page=3&pageSize=3").TryGetAsObjectAsync<WeatherForecast[]>(),
            };
        }
    }
}