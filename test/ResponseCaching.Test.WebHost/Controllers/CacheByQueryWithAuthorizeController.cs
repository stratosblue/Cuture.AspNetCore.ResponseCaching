﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using ResponseCaching.Test.WebHost.Models;
using ResponseCaching.Test.WebHost.Test;

namespace ResponseCaching.Test.WebHost.Controllers
{
    public class CacheByQueryWithAuthorizeController : TestControllerBase
    {
        #region Private 字段

        private readonly ILogger _logger;

        #endregion Private 字段

        #region Public 构造函数

        public CacheByQueryWithAuthorizeController(ILogger<CacheByQueryWithAuthorizeController> logger)
        {
            _logger = logger;
        }

        #endregion Public 构造函数

        #region Public 方法

        [HttpGet]
        [AuthorizeMixed]
        [CacheByQuery(Duration,
                      "page", "pageSize")]
        [ExecutingLock(ExecutingLockMode.CacheKeySingle)]
        public IEnumerable<WeatherForecast> Get([Required] int page, [Required] int pageSize)
        {
            _logger.LogInformation("{0} - {1}", page, pageSize);
            return TestDataGenerator.GetData((page - 1) * pageSize, pageSize);
        }

        #endregion Public 方法
    }
}