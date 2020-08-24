using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Cuture.Http;

using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost;

namespace ResponseCaching.Test.Base
{
    [TestClass]
    public abstract class WebServerHostedTestBase
    {
        public string BaseUrl { get; set; } = "http://127.0.0.1:18080";

        protected IHost WebHost { get; private set; }

        [TestInitialize]
        public virtual async Task InitAsync()
        {
            var hostBuilder = TestWebHost.CreateHostBuilder(GetHostArgs());
            await ConfigureWebHost(hostBuilder);
            WebHost = await hostBuilder.StartAsync();

            HttpRequestOptions.DefaultConnectionLimit = 500;
        }

        [TestCleanup]
        public virtual async Task CleanupAsync()
        {
            await WebHost.StopAsync();
        }

        protected virtual Task ConfigureWebHost(IHostBuilder hostBuilder) => Task.CompletedTask;

        protected virtual string[] GetHostArgs() => new[] { "--urls", BaseUrl };

        /// <summary>
        /// 获取一个大小为 <paramref name="count"/> 的数组
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        protected int[] Array(int count) => new int[count];

        /// <summary>
        /// 并行请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestCount">请求总数</param>
        /// <param name="getRequestFunc">获取请求的方法</param>
        /// <param name="assertAction">对每个请求的断言委托</param>
        /// <returns></returns>
        protected virtual async Task ParallelRequestAsync<T>(int requestCount,
                                                             Func<IHttpTurboRequest> getRequestFunc,
                                                             Action<HttpOperationResult<T>> assertAction)
        {
            if (requestCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(requestCount));
            }

            if (getRequestFunc is null)
            {
                throw new ArgumentNullException(nameof(getRequestFunc));
            }

            if (assertAction is null)
            {
                throw new ArgumentNullException(nameof(assertAction));
            }

            Debug.WriteLine($"Start Request, Count: {requestCount}");

            var sw = Stopwatch.StartNew();

            var tasks = Array(requestCount).Select(m => ConfigureBeforeRequest(getRequestFunc()).TryGetAsObjectAsync<T>()).ToList();

            await Task.WhenAll(tasks);

            sw.Stop();

            Debug.WriteLine($"Total Time: {sw.Elapsed.TotalSeconds} s");

            tasks.ForEach(m =>
            {
                assertAction(m.Result);
            });
        }

        protected virtual IHttpTurboRequest ConfigureBeforeRequest(IHttpTurboRequest request) => request;
    }
}