using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.Interceptors;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;

namespace ResponseCaching.Test.WebHost.Test
{
    public class TestCachingProcessInterceptor : CachingProcessInterceptor
    {
        #region Public 方法

        public override Task<ResponseCacheEntry> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate next)
        {
            return Task.FromResult<ResponseCacheEntry>(null);
        }

        #endregion Public 方法
    }
}