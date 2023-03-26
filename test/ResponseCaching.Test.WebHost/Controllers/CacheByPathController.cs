using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using ResponseCaching.Test.WebHost.Models;
using ResponseCaching.Test.WebHost.Test;

namespace ResponseCaching.Test.WebHost.Controllers;

public class CacheByPathController : TestControllerBase
{
    #region Private 字段

    private readonly ILogger _logger;

    #endregion Private 字段

    #region Public 构造函数

    public CacheByPathController(ILogger<CacheByPathController> logger)
    {
        _logger = logger;
    }

    #endregion Public 构造函数

    #region Public 方法

    [HttpGet]
    [CacheByPath(Duration)]
    [ExecutingLock(ExecutingLockMode.CacheKeySingle)]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("{0}", "path-cache");
        return TestDataGenerator.GetData(1, 5);
    }

    [HttpPost]
    [CacheByPath(Duration)]
    [ExecutingLock(ExecutingLockMode.CacheKeySingle)]
    public IEnumerable<WeatherForecast> Post()
    {
        _logger.LogInformation("{0}", "path-cache");
        return TestDataGenerator.GetData(1, 5);
    }

    #endregion Public 方法
}
