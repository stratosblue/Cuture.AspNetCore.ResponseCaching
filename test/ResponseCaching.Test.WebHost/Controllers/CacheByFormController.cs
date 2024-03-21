using Microsoft.AspNetCore.Mvc;

using ResponseCaching.Test.WebHost.Models;
using ResponseCaching.Test.WebHost.Test;

namespace ResponseCaching.Test.WebHost.Controllers;

public class CacheByFormController : TestControllerBase
{
    #region Private 字段

    private readonly ILogger _logger;

    #endregion Private 字段

    #region Public 构造函数

    public CacheByFormController(ILogger<CacheByFormController> logger)
    {
        _logger = logger;
    }

    #endregion Public 构造函数

    #region Public 方法

    [HttpPost]
    [CacheByForm(Duration, "page", "pageSize")]
    [ExecutingLock(ExecutingLockMode.CacheKeySingle)]
    public IEnumerable<WeatherForecast> Post()
    {
        int page = int.Parse(Request.Form["page"]);
        int pageSize = int.Parse(Request.Form["pageSize"]);
        _logger.LogInformation("{0} - {1}", page, pageSize);
        return TestDataGenerator.GetData((page - 1) * pageSize, pageSize);
    }

    #endregion Public 方法
}
