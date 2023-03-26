using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using ResponseCaching.Test.WebHost.Models;
using ResponseCaching.Test.WebHost.Test;

namespace ResponseCaching.Test.WebHost.Controllers;

public class CacheByHeaderController : TestControllerBase
{
    #region Private 字段

    private readonly ILogger _logger;

    #endregion Private 字段

    #region Public 构造函数

    public CacheByHeaderController(ILogger<CacheByHeaderController> logger)
    {
        _logger = logger;
    }

    #endregion Public 构造函数

    #region Public 方法

    [HttpGet]
    [CacheByHeader(Duration,
                   "page", "pageSize")]
    [ExecutingLock(ExecutingLockMode.CacheKeySingle)]
    public IEnumerable<WeatherForecast> Get()
    {
        int page = int.Parse(Request.Headers["page"]);
        int pageSize = int.Parse(Request.Headers["pageSize"]);
        _logger.LogInformation("{0} - {1}", page, pageSize);
        return TestDataGenerator.GetData((page - 1) * pageSize, pageSize);
    }

    #endregion Public 方法
}