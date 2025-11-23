using System.ComponentModel.DataAnnotations;

using Cuture.AspNetCore.ResponseCaching;

using Microsoft.AspNetCore.Mvc;

using ResponseCaching.Test.WebHost.Models;
using ResponseCaching.Test.WebHost.Test;

namespace ResponseCaching.Test.WebHost.Controllers;

public class ExecuteLockTimeoutController : TestControllerBase
{
    #region Private 字段

    private readonly ILogger _logger;

    #endregion Private 字段

    #region Public 构造函数

    public ExecuteLockTimeoutController(ILogger<ExecuteLockTimeoutController> logger)
    {
        _logger = logger;
    }

    #endregion Public 构造函数

    #region Public 方法

    [HttpGet]
    [CacheByModel(Duration)]
    [ExecutingLock(ExecutingLockMode.ActionSingle, 500)]
    public async Task<IEnumerable<WeatherForecast>> ActionFilterAsync([Required][FromQuery] Input input)
    {
        _logger.LogInformation("Wait {ActionFilterAsync} - {InputWaitMilliseconds}", nameof(ActionFilterAsync), input.WaitMilliseconds);
        await Task.Delay(input.WaitMilliseconds);
        return TestDataGenerator.GetData(1, 5);
    }

    [HttpGet]
    [ResponseCaching(Duration, Mode = CacheMode.PathUniqueness)]
    [ExecutingLock(ExecutingLockMode.ActionSingle, 500)]
    public async Task<IEnumerable<WeatherForecast>> ResourceFilterAsync([Required][FromQuery] int waitMilliseconds)
    {
        _logger.LogInformation("Wait {ResourceFilterAsync} - {WaitMilliseconds}", nameof(ResourceFilterAsync), waitMilliseconds);
        await Task.Delay(waitMilliseconds);
        return TestDataGenerator.GetData(1, 5);
    }

    #endregion Public 方法

    #region Public 类

    public class Input : ICacheKeyable
    {
        #region Public 属性

        public int WaitMilliseconds { get; set; }

        #endregion Public 属性

        #region Public 方法

        public string AsCacheKey() => string.Empty;

        #endregion Public 方法
    }

    #endregion Public 类
}
