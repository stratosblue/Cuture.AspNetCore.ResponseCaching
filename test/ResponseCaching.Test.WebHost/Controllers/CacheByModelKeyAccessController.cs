using Cuture.AspNetCore.ResponseCaching;
using Microsoft.AspNetCore.Mvc;

using ResponseCaching.Test.WebHost.Dtos;

namespace ResponseCaching.Test.WebHost.Controllers;

public class CacheByModelKeyAccessController : TestControllerBase
{
    #region Private 字段

    private readonly ICacheKeyAccessor _cacheKeyAccessor;

    private readonly ILogger _logger;

    #endregion Private 字段

    #region Public 构造函数

    public CacheByModelKeyAccessController(ILogger<CacheByModelController> logger, ICacheKeyAccessor cacheKeyAccessor)
    {
        _logger = logger;
        _cacheKeyAccessor = cacheKeyAccessor;
    }

    #endregion Public 构造函数

    #region Public 方法

    [HttpPost]
    [CacheByModel(Duration, "input")]
    [ExecutingLock(ExecutingLockMode.ActionSingle)]
    public string Post(PageResultRequestDto input)
    {
        return _cacheKeyAccessor.Key!;
    }

    #endregion Public 方法
}
