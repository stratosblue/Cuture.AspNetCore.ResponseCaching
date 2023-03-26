using System;
using System.Linq;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.Base;
using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class MaxCacheableResponseLengthTest : WebServerHostedTestBase
{
    #region Public 方法

    [TestMethod]
    public async Task ShouldActionFilterNotCacheAsync()
    {
        await ExecuteAsync("by-action-filter");
    }

    [TestMethod]
    public async Task ShouldResourceFilterNotCacheAsync()
    {
        await ExecuteAsync("by-resource-filter");
    }

    #endregion Public 方法

    #region Private 方法

    private async Task ExecuteAsync(string actionName)
    {
        var cacheableUrl = $"{BaseUrl}/MaxCacheableResponseLength/{actionName}?count=1";
        var notCacheableUrl = $"{BaseUrl}/MaxCacheableResponseLength/{actionName}?count=20";

        var cachedData = await InternalRunAsync(Enumerable.Range(0, 100).Select(_ => GetRequestFuncs(cacheableUrl)).ToArray());

        CheckForEachOther(cachedData, true);

        var notCachedData = await InternalRunAsync(Enumerable.Range(0, 100).Select(_ => GetRequestFuncs(notCacheableUrl)).ToArray());

        CheckForEachOther(notCachedData, false);

        Func<Task<TextHttpOperationResult<WeatherForecast[]>>> GetRequestFuncs(string url)
        {
            return () => url.CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>();
        }
    }

    #endregion Private 方法
}