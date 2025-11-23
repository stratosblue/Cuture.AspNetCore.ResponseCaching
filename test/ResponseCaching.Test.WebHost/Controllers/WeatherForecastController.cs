using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using ResponseCaching.Test.WebHost.Models;
using ResponseCaching.Test.WebHost.Test;

namespace ResponseCaching.Test.WebHost.Controllers;

public class WeatherForecastController : TestControllerBase
{
    #region Private 字段

    private readonly ILogger _logger;

    #endregion Private 字段

    #region Public 构造函数

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    #endregion Public 构造函数

    #region Public 方法

    [HttpGet]
    public IEnumerable<WeatherForecast> Get([Required] int page, [Required] int pageSize)
    {
        _logger.LogInformation("{Page} - {PageSize}", page, pageSize);
        return TestDataGenerator.GetData((page - 1) * pageSize, pageSize);
    }

    #endregion Public 方法
}
