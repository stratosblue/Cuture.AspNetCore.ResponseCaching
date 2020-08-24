using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using ResponseCaching.Test.WebHost.Dtos;
using ResponseCaching.Test.WebHost.Models;
using ResponseCaching.Test.WebHost.Test;

namespace ResponseCaching.Test.WebHost.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        private const int Duration = 10;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ActionName("get")]
        public IEnumerable<WeatherForecast> Get([Required] int page, [Required] int pageSize)
        {
            _logger.LogInformation("{0} : {1}", DateTime.Now, "get");
            return TestDataGenerator.GetData((page - 1) * pageSize, pageSize);
        }

        [HttpGet]
        [ActionName("get-q")]
        [CacheByQuery(Duration, "page", "pageSize", LockMode = ExecutingLockMode.CacheKeySingle)]
        public IEnumerable<WeatherForecast> CacheByQuery([Required] int page, [Required] int pageSize)
        {
            _logger.LogInformation("{0} : {1}", DateTime.Now, "get-q");
            return TestDataGenerator.GetData((page - 1) * pageSize, pageSize);
        }

        [HttpPost]
        [ActionName("get-f")]
        [CacheByForm(Duration, "page", "pageSize", LockMode = ExecutingLockMode.CacheKeySingle)]
        public IEnumerable<WeatherForecast> CacheByForm()
        {
            int page = int.Parse(Request.Form["page"]);
            int pageSize = int.Parse(Request.Form["pageSize"]);
            _logger.LogInformation("{0} : {1}", DateTime.Now, "get-f");
            return TestDataGenerator.GetData((page - 1) * pageSize, pageSize);
        }

        [HttpGet]
        [ActionName("get-h")]
        [CacheByHeader(Duration,
                       "page", "pageSize",
                       LockMode = ExecutingLockMode.CacheKeySingle)]
        public IEnumerable<WeatherForecast> CacheByHeader()
        {
            int page = int.Parse(Request.Headers["page"]);
            int pageSize = int.Parse(Request.Headers["pageSize"]);
            _logger.LogInformation("{0} : {1}", DateTime.Now, "get-h");
            return TestDataGenerator.GetData((page - 1) * pageSize, pageSize);
        }

        [HttpPost]
        [ActionName("get-m")]
        [CacheByModel(Duration,
                      "input",
                      LockMode = ExecutingLockMode.CacheKeySingle)]
        public IEnumerable<WeatherForecast> CacheByModel(PageResultRequestDto input)
        {
            int page = input.Page;
            int pageSize = input.PageSize;
            _logger.LogInformation("{0} : {1}", DateTime.Now, "get-m");
            return TestDataGenerator.GetData((page - 1) * pageSize, pageSize);
        }

        [HttpGet]
        [AuthorizeMixed]
        [ActionName("get-at-q")]
        [CacheByQuery(Duration,
                      "page", "pageSize",
                      LockMode = ExecutingLockMode.CacheKeySingle)]
        public IEnumerable<WeatherForecast> CacheByQueryWithAuthorize([Required] int page, [Required] int pageSize)
        {
            _logger.LogInformation("{0} : {1}", DateTime.Now, "get-at-q");
            return TestDataGenerator.GetData((page - 1) * pageSize, pageSize);
        }

        [HttpPost]
        [AuthorizeMixed]
        [ActionName("get-all-mixed")]
        [ResponseCaching(Duration,
                         Mode = CacheMode.Custom,
                         VaryByClaims = new[] { "id", "sid" },
                         VaryByHeaders = new[] { "page", "pageSize" },
                         VaryByQueryKeys = new[] { "page", "pageSize" },
                         VaryByModels = new[] { "input" },
                         LockMode = ExecutingLockMode.CacheKeySingle)]
        public IEnumerable<WeatherForecast> CacheByAllMixed([Required][FromQuery] int page, [Required][FromQuery] int pageSize, [FromBody] PageResultRequestDto input)
        {
            _logger.LogInformation("{0} : {1}", DateTime.Now, "get-all-mixed");
            return TestDataGenerator.GetData((page - 1) * pageSize, pageSize);
        }
    }
}