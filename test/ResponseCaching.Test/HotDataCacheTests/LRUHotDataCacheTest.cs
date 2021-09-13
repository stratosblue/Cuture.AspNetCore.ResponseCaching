using System.Linq;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;
using Cuture.Http;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.Base;

namespace ResponseCaching.Test.RequestTests
{
    [TestClass]
    public class LRUHotDataCacheTest : WebServerHostedTestBase
    {
        private readonly CountDistributedResponseCache _countDistributedResponseCache = new();

        protected override Task ConfigureWebHost(IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services => services.AddSingleton<IDistributedResponseCache>(_countDistributedResponseCache));
            return base.ConfigureWebHost(hostBuilder);
        }

        [TestMethod]
        public async Task None_DistributedResponseCache_Get()
        {
            var urls = Enumerable.Range(0, 50).Select(m => $"{BaseUrl}/HotDataCache/Get?input={m}").ToArray();

            var tasks = urls.Select(url => Enumerable.Range(0, 100).Select(_ => url.CreateHttpRequest().TryGetAsStringAsync())).SelectMany(m => m).ToArray();

            await Task.WhenAll(tasks);

            Assert.AreEqual(0, tasks.Count(m => !m.Result.IsSuccessStatusCode));

            Assert.AreEqual(0, _countDistributedResponseCache.Count(null));
        }

        [TestMethod]
        public async Task HotDataCache_Replacement()
        {
            var urlCount = 50;
            var rCount = 25;
            var urls = Enumerable.Range(0, urlCount).Select(m => $"{BaseUrl}/HotDataCache/Get?input={m}").ToArray();
            var rUrls = Enumerable.Range(urlCount * 2, rCount).Select(m => $"{BaseUrl}/HotDataCache/Get?input={m}").ToArray();

            //确保缓存是有序的
            foreach (var item in urls)
            {
                var httpOperationResult = await item.CreateHttpRequest().TryGetAsStringAsync();
                Assert.IsTrue(httpOperationResult.IsSuccessStatusCode);
            }

            var tasks = urls.Select(url => Enumerable.Range(0, 100).Select(_ => url.CreateHttpRequest().TryGetAsStringAsync())).SelectMany(m => m).ToArray();

            await Task.WhenAll(tasks);

            Assert.AreEqual(0, tasks.Count(m => !m.Result.IsSuccessStatusCode));

            Assert.AreEqual(0, _countDistributedResponseCache.Count(null));

            //将缓存过期 rCount 个
            tasks = rUrls.Select(url => url.CreateHttpRequest().TryGetAsStringAsync()).ToArray();
            await Task.WhenAll(tasks);
            Assert.AreEqual(0, tasks.Count(m => !m.Result.IsSuccessStatusCode));
            Assert.AreEqual(0, _countDistributedResponseCache.Count(null));

            //确认前 rCount 个缓存已经过期
            //先顺序执行，避免并发访问时穿透本地缓存请求到分布式缓存
            foreach (var item in urls.Take(rCount))
            {
                var httpOperationResult = await item.CreateHttpRequest().TryGetAsStringAsync();
                Assert.IsTrue(httpOperationResult.IsSuccessStatusCode);
            }

            tasks = urls.Take(rCount).Select(url => Enumerable.Range(0, 100).Select(_ => url.CreateHttpRequest().TryGetAsStringAsync())).SelectMany(m => m).ToArray();

            await Task.WhenAll(tasks);

            Assert.AreEqual(0, tasks.Count(m => !m.Result.IsSuccessStatusCode));

            Assert.AreEqual(urlCount - rCount, _countDistributedResponseCache.Count(null));

            foreach (var item in _countDistributedResponseCache.GetKeys())
            {
                Assert.AreEqual(1, _countDistributedResponseCache.Count(item));
            }
        }
    }
}
