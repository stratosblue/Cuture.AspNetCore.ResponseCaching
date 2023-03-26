using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class CacheByCustomTest : BaseRequestTest
{
    protected override Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] GetAllRequestFuncs()
    {
        return new Func<Task<TextHttpOperationResult<WeatherForecast[]>>>[] {
            () => $"{BaseUrl}/CacheByCustomCacheKeyGenerator/Get?page=1&pageSize=5".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByCustomCacheKeyGenerator/Get?page=1&pageSize=6".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByCustomCacheKeyGenerator/Get?page=2&pageSize=4".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByCustomCacheKeyGenerator/Get?page=2&pageSize=6".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
            () => $"{BaseUrl}/CacheByCustomCacheKeyGenerator/Get?page=3&pageSize=3".CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>(),
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
                AreEqual(data[i], data[j]);
            }
        }

        for (int time = 0; time < ReRequestTimes; time++)
        {
            var values = await InternalRunAsync(funcs);

            Assert.AreEqual(data.Length, values.Length);

            for (int i = 0; i < values.Length; i++)
            {
                var target = data[i];
                var value = values[i];
                Assert.AreEqual(target.Length, value.Length);

                for (int index = 0; index < target.Length; index++)
                {
                    Debug.WriteLine($"index-{index}: {target[index]} --- {value[index]}");
                    Assert.IsTrue(target[index].Equals(value[index]));
                }
            }
        }
    }
}
