using System.Collections.Generic;
using System.ComponentModel;

using Cuture.AspNetCore.ResponseCaching;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using ResponseCaching.Test.WebHost.Models;
using ResponseCaching.Test.WebHost.Test;

namespace ResponseCaching.Test.WebHost.Controllers;

public class CacheByCustomCacheKeyGeneratorController : TestControllerBase
{
    #region Private 字段

    private readonly ILogger _logger;

    #endregion Private 字段

    #region Public 构造函数

    public CacheByCustomCacheKeyGeneratorController(ILogger<CacheByCustomCacheKeyGeneratorController> logger)
    {
        _logger = logger;
    }

    #endregion Public 构造函数

    #region Public 方法

    [HttpGet]
    [ResponseCaching(Duration)]
    [ExecutingLock(ExecutingLockMode.CacheKeySingle)]
    [Description("cache_key_definite")]
    [CacheKeyGenerator(typeof(TestCustomCacheKeyGenerator), FilterType.Resource)]
    public IEnumerable<WeatherForecast> Get()
    {
        return TestDataGenerator.GetData(0, 5);
    }

    #endregion Public 方法
}