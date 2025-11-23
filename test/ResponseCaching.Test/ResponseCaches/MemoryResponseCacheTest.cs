using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.Extensions.Logging;

namespace ResponseCaching.Test.ResponseCaches;

[TestClass]
public class MemoryResponseCacheTest : ResponseCacheTest
{
    protected override Task<IResponseCache> GetResponseCache()
    {
        return Task.FromResult<IResponseCache>(new DefaultMemoryResponseCache(new LoggerFactory()));
    }
}
