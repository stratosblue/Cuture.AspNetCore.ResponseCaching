using System;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByFullQueryTest : BaseRequestTest
{
    #region Protected 方法

    protected override Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
    {
        return new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByFullQuery/Get?page=1&pageSize=3".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?page=1&pageSize=3&t".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?page=1&pageSize=3&=t".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?page=1&pageSize=3&=0".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?page=1&pageSize=3&t=0".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
        };
    }

    #endregion Protected 方法
}
