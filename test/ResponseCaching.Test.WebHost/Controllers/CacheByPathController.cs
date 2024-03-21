using Microsoft.AspNetCore.Mvc;

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
        _logger.LogInformation("{0}", "path-cache Get");
        return TestDataGenerator.GetData(1, 5);
    }

    [HttpPost]
    [CacheByPath(Duration)]
    [ExecutingLock(ExecutingLockMode.CacheKeySingle)]
    public IEnumerable<WeatherForecast> Post()
    {
        _logger.LogInformation("{0}", "path-cache Post");
        return TestDataGenerator.GetData(1, 5);
    }

    #region Route

    [HttpGet]
    [CacheByPath(Duration)]
    [ExecutingLock(ExecutingLockMode.CacheKeySingle)]
    [Route("/R1/{Value1}")]
    public IEnumerable<WeatherForecast> AbsoluteRoute1()
    {
        _logger.LogInformation("{0}", "path-cache AbsoluteRoute1");
        return TestDataGenerator.GetData(1, 5);
    }

    [HttpGet]
    [CacheByPath(Duration)]
    [ExecutingLock(ExecutingLockMode.CacheKeySingle)]
    [Route("R1/{Value1}")]
    public IEnumerable<WeatherForecast> RelativeRoute1()
    {
        _logger.LogInformation("{0}", "path-cache RelativeRoute1");
        return TestDataGenerator.GetData(1, 5);
    }

    #endregion Route

    #endregion Public 方法
}
