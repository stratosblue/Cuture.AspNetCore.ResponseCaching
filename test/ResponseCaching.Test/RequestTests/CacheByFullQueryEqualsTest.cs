using System;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByFullQueryEqualsTest : BaseRequestTest
{
    #region Protected 属性

    protected override bool ShouldEqualEachOtherRequest => true;

    #endregion Protected 属性

    #region Protected 方法

    protected override Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
    {
        return new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByFullQuery/Get?page=1&pageSize=3&t=1&t3=1&t2=1&t800".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?t=1&t3=1&t2=1&t800&page=1&pageSize=3".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?t=1&page=1&t3=1&t800&pageSize=3&t2=1".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?t3=1&page=1&t=1&t2=1&t800&pageSize=3".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByFullQuery/Get?t2=1&page=1&pageSize=3&t=1&t3=1&t800".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
        };
    }

    #endregion Protected 方法
}
