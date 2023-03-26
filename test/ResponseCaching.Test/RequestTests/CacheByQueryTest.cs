using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByQueryTest : BaseRequestTest
{
    protected override Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
    {
        return new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByQuery/Get?page=1&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQuery/Get?page=1&pageSize=6".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQuery/Get?page=2&pageSize=4".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQuery/Get?page=2&pageSize=6".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByQuery/Get?page=3&pageSize=3".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
        };
    }
}
