using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;

namespace ResponseCaching.Test.WebHost.Controllers;

public class HotDataCacheController : TestControllerBase
{
    #region Public 方法

    [HttpGet]
    [HotDataCache(50, HotDataCachePolicy.LRU)]
    [ResponseCaching(3,
        Mode = CacheMode.FullPathAndQuery,
        StoreLocation = CacheStoreLocation.Distributed)]
    [ExecutingLock(ExecutingLockMode.CacheKeySingle)]
    public string Get(string input)
    {
        return "Inputed:" + input;
    }

    #endregion Public 方法
}