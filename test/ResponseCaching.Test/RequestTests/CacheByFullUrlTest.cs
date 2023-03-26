using System;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByFullUrlTest : BaseRequestTest
{
    protected override int ReRequestTimes => 4;

    private readonly long _t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    protected override Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
    {
        return new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByFullUrl/Get?page=1&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullUrl/Get?page=2&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullUrl/Get?page=3&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullUrl/Get?page=1&pageSize=5&_t=1".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullUrl/Get?page=1&pageSize=5&_t={_t}".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullUrl/Get?page=1&pageSize=5&_t=1&_t1=0".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
        };
    }
}
