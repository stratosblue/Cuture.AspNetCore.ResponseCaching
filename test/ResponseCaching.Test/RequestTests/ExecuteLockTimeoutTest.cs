using Cuture.Http;

using ResponseCaching.Test.Base;
using ResponseCaching.Test.WebHost.Models;

namespace ResponseCaching.Test.RequestTests;

[TestClass]
public class ExecuteLockTimeoutTest : WebServerHostedTestBase
{
    #region Public 方法

    [TestMethod]
    public async Task ShouldActionFilterReturn429Async()
    {
        await ExecuteAsync("ActionFilter");
    }

    [TestMethod]
    public async Task ShouldResourceFilterReturn429Async()
    {
        await ExecuteAsync("ResourceFilter");
    }

    #endregion Public 方法

    #region Private 方法

    private async Task ExecuteAsync(string actionName)
    {
        var waitUrl = $"{BaseUrl}/ExecuteLockTimeout/{actionName}?waitMilliseconds=2000";
        var noWaitUrl = $"{BaseUrl}/ExecuteLockTimeout/{actionName}?waitMilliseconds=0";

        var waitTask = GetRequestTasks(waitUrl);

        //延时
        await Task.Delay(200);

        var noWaitTasks = Enumerable.Range(0, 50).Select(_ => GetRequestTasks(noWaitUrl)).ToArray();

        await Task.WhenAll(noWaitTasks);

        await waitTask;

        Assert.IsTrue(waitTask.IsCompletedSuccessfully);

        Assert.IsGreaterThan(0, waitTask.Result?.Data?.Length ?? 0);

        for (int i = 0; i < noWaitTasks.Length; i++)
        {
            var item = noWaitTasks[i];
            Assert.IsTrue(item.IsCompletedSuccessfully, $"fail at {i}");
            Assert.IsNull(item.Result?.Data, $"fail at {i}");
            Assert.IsNotNull(item.Result?.ResponseMessage, $"fail at {i}");

            Assert.AreEqual(System.Net.HttpStatusCode.TooManyRequests, item.Result.ResponseMessage.StatusCode, $"fail at {i}");
        }

        Task<TextHttpOperationResult<WeatherForecast[]>> GetRequestTasks(string url)
        {
            return url.CreateHttpRequest().TryGetAsObjectAsync<WeatherForecast[]>();
        }
    }

    #endregion Private 方法
}
