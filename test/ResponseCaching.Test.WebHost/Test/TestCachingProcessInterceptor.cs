using Cuture.AspNetCore.ResponseCaching.Interceptors;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;

namespace ResponseCaching.Test.WebHost.Test;

public class TestCachingProcessInterceptor : Attribute, ICacheStoringInterceptor
{
    #region Public 方法

    public Task<ResponseCacheEntry> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate<ActionContext> next)
    {
        return Task.FromResult<ResponseCacheEntry>(null);
    }

    #endregion Public 方法
}
