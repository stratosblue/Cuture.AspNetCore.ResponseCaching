using System;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByHeaderTest : BaseRequestTest
{
    protected override Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
    {
        return new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByHeader/Get".CreateHttpRequest().AddHeader("page","1").AddHeader("pageSize","5").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByHeader/Get".CreateHttpRequest().AddHeader("page","1").AddHeader("pageSize","6").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByHeader/Get".CreateHttpRequest().AddHeader("page","2").AddHeader("pageSize","4").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByHeader/Get".CreateHttpRequest().AddHeader("page","2").AddHeader("pageSize","6").TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByHeader/Get".CreateHttpRequest().AddHeader("page","3").AddHeader("pageSize","3").TryGetAsObjectAsync<WeatherForecast[]>(),
        };
    }
}
