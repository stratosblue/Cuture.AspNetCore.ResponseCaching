using Microsoft.AspNetCore.Mvc;

using ResponseCaching.Test.WebHost.Dtos;
using ResponseCaching.Test.WebHost.Models;
using ResponseCaching.Test.WebHost.Test;

namespace ResponseCaching.Test.WebHost.Controllers;

public class CacheByModelController : TestControllerBase
{
    #region Private 字段

    private readonly ILogger _logger;

    #endregion Private 字段

    #region Public 构造函数

    public CacheByModelController(ILogger<CacheByModelController> logger)
    {
        _logger = logger;
    }

    #endregion Public 构造函数

    #region Public 方法

    [HttpPost]
    [CacheByModel(Duration, "input")]
    [ExecutingLock(ExecutingLockMode.ActionSingle)]
    public IEnumerable<WeatherForecast> Post(PageResultRequestDto input)
    {
        int page = input.Page;
        int pageSize = input.PageSize;
        _logger.LogInformation("{Page} - {PageSize}", page, pageSize);
        return TestDataGenerator.GetData((page - 1) * pageSize, pageSize);
    }

    #endregion Public 方法
}
