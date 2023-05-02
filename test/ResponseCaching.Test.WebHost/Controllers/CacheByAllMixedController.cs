using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using ResponseCaching.Test.WebHost.Dtos;
using ResponseCaching.Test.WebHost.Models;
using ResponseCaching.Test.WebHost.Test;

namespace ResponseCaching.Test.WebHost.Controllers;

public class CacheByAllMixedController : TestControllerBase
{
    #region Private 字段

    private readonly ILogger _logger;

    #endregion Private 字段

    #region Public 构造函数

    public CacheByAllMixedController(ILogger<CacheByAllMixedController> logger)
    {
        _logger = logger;
    }

    #endregion Public 构造函数

    #region Public 方法

    [HttpPost]
    [AuthorizeMixed]
    [ResponseCaching(Duration,
                     Mode = CacheMode.Custom,
                     VaryByClaims = new[] { "id", "sid" },
                     VaryByHeaders = new[] { "page", "pageSize" },
                     VaryByQueryKeys = new[] { "page", "pageSize" },
                     VaryByModels = new[] { "input" })]
    [ExecutingLock(ExecutingLockMode.CacheKeySingle)]
    public IEnumerable<WeatherForecast> Post([Required][FromQuery] int page, [Required][FromQuery] int pageSize, [FromBody] PageResultRequestDto input)
    {
        _logger.LogInformation("{0} - {1}", page, pageSize);
        return TestDataGenerator.GetData((page - 1) * pageSize, pageSize);
    }

    [HttpPost]
    [AuthorizeMixed]
    [ResponseCaching(Duration,
                 Mode = CacheMode.Custom,
                 VaryByClaims = new[] { "id", "sid" },
                 VaryByHeaders = new[] { "page", "pageSize" },
                 VaryByQueryKeys = new[] { "page", "pageSize" },
                 VaryByModels = new[] { "input" })]
    [ExecutingLock(ExecutingLockMode.CacheKeySingle)]
    [Route("{Value1}/{Value2}")]
    public IEnumerable<WeatherForecast> PostWithPath([Required][FromQuery] int page, [Required][FromQuery] int pageSize, [FromBody] PageResultRequestDto input)
    {
        _logger.LogInformation("{0} - {1}", page, pageSize);
        return TestDataGenerator.GetData((page - 1) * pageSize, pageSize);
    }

    #endregion Public 方法
}
