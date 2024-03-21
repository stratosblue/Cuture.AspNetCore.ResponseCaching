using System.ComponentModel.DataAnnotations;
using Cuture.AspNetCore.ResponseCaching;
using Microsoft.AspNetCore.Mvc;

namespace ResponseCaching.Test.WebHost.Controllers;

public class CacheByFullUrlKeyAccessController : TestControllerBase
{
    #region Private 字段

    private readonly ICacheKeyAccessor _cacheKeyAccessor;

    private readonly ILogger _logger;

    #endregion Private 字段

    #region Public 构造函数

    public CacheByFullUrlKeyAccessController(ILogger<CacheByFullUrlController> logger, ICacheKeyAccessor cacheKeyAccessor)
    {
        _logger = logger;
        _cacheKeyAccessor = cacheKeyAccessor;
    }

    #endregion Public 构造函数

    #region Public 方法

    [HttpGet]
    [CacheByFullUrl(Duration)]
    [ExecutingLock(ExecutingLockMode.ActionSingle)]
    [ResponseDumpCapacity(128)]
    public string Get([Required] int page, [Required] int pageSize)
    {
        return _cacheKeyAccessor.Key!;
    }

    #endregion Public 方法
}
