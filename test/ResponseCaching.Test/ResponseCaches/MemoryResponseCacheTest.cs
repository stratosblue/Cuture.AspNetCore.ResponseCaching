using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ResponseCaching.Test.ResponseCaches
{
    [TestClass]
    public class MemoryResponseCacheTest : ResponseCacheTest
    {
        protected override Task<IResponseCache> GetResponseCache()
        {
            return Task.FromResult<IResponseCache>(new DefaultMemoryResponseCache(new LoggerFactory()));
        }
    }
}