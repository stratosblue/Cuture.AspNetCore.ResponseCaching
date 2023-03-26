using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using ResponseCaching.Test.WebHost.Models;
using ResponseCaching.Test.WebHost.Test;

namespace ResponseCaching.Test.WebHost.Controllers;

public class MaxCacheableResponseLengthController : TestControllerBase
{
    #region Private 字段

    private readonly ILogger _logger;

    #endregion Private 字段

    #region Public 构造函数

    public MaxCacheableResponseLengthController(ILogger<MaxCacheableResponseLengthController> logger)
    {
        _logger = logger;
    }

    #endregion Public 构造函数

    #region Public 方法

    [HttpGet]
    [ActionName("by-action-filter")]
    [CacheByModel(Duration, "count", MaxCacheableResponseLength = 256)]
    [ExecutingLock(ExecutingLockMode.CacheKeySingle)]
    public IEnumerable<WeatherForecast> ByActionFilter(int count)
    {
        _logger.LogInformation(count.ToString());
        return TestDataGenerator.GetData(1, count);
    }

    [HttpGet]
    [ActionName("by-resource-filter")]
    [CacheByQuery(Duration, "count", MaxCacheableResponseLength = 256)]
    [ExecutingLock(ExecutingLockMode.CacheKeySingle)]
    public IEnumerable<WeatherForecast> ByResourceFilter(int count)
    {
        _logger.LogInformation(count.ToString());
        return TestDataGenerator.GetData(1, count);
    }

    #endregion Public 方法
}
