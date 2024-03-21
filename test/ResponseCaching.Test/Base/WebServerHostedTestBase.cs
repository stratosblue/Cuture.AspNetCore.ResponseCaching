using System.Diagnostics;

using Cuture.Http;

using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ResponseCaching.Test.WebHost;

namespace ResponseCaching.Test.Base;

[TestClass]
public abstract class WebServerHostedTestBase
{
    #region Public 属性

    public string BaseUrl { get; set; } = "http://127.0.0.1:18080";

    #endregion Public 属性

    #region Protected 属性

    protected IHost WebHost { get; private set; }

    #endregion Protected 属性

    #region Public 方法

    [TestCleanup]
    public virtual async Task CleanupAsync()
    {
        if (WebHost != null)
        {
            await WebHost.StopAsync();
            WebHost.Dispose();
        }
    }

    [TestInitialize]
    public virtual async Task InitAsync()
    {
        TestWebHost.IsTest = true;
        var hostBuilder = TestWebHost.CreateHostBuilder(GetHostArgs());
        await ConfigureWebHost(hostBuilder);
        WebHost = await hostBuilder.StartAsync();

        HttpRequestGlobalOptions.DefaultHttpMessageInvokerPool = new TestCutureHttpMessageInvokerPool(WebHost.GetTestClient());

        HttpRequestGlobalOptions.DefaultConnectionLimit = 500;
    }

    #endregion Public 方法

    #region Protected 方法

    protected void AreEqual<T>(IEnumerable<T> items1, IEnumerable<T> items2) where T : IEquatable<T>
    {
        if (items1 != null
            && items2 != null)
        {
            var count1 = items1.Count();
            var count2 = items2.Count();

            Assert.AreEqual(count1, count2);

            using var enumerator1 = items1.GetEnumerator();
            using var enumerator2 = items2.GetEnumerator();

            var index = 0;

            while (enumerator1.MoveNext()
                   && enumerator2.MoveNext())
            {
                Assert.IsNotNull(enumerator1.Current);
                Assert.IsTrue(enumerator1.Current.Equals(enumerator2.Current), $"index: {index} - Item1: {enumerator1.Current} Item2: {enumerator2.Current}");
            }
        }
    }

    protected void AreNotEqual<T>(IEnumerable<T> items1, IEnumerable<T> items2) where T : IEquatable<T>
    {
        if (items1 != null
            && items2 != null)
        {
            var count1 = items1.Count();
            var count2 = items2.Count();

            if (count1 != count2)
            {
                return;
            }

            using var enumerator1 = items1.GetEnumerator();
            using var enumerator2 = items2.GetEnumerator();

            bool allSame = true;
            while (enumerator1.MoveNext()
                   && enumerator2.MoveNext())
            {
                allSame = enumerator1.Current.Equals(enumerator2.Current);
                if (!allSame)
                {
                    break;
                }
            }
            Assert.AreNotEqual(true, allSame, "两个序列数据相同");
        }
    }

    /// <summary>
    /// 获取一个大小为 <paramref name="count"/> 的数组
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    protected int[] Array(int count) => new int[count];

    /// <summary>
    /// 对比同一批请求中的响应是否相同
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="equal">是否应该相同</param>
    protected void CheckForEachOther<T>(T[][] data, bool equal) where T : IEquatable<T>
    {
        for (int i = 0; i < data.Length - 1; i++)
        {
            for (int j = i + 1; j < data.Length; j++)
            {
                if (equal)
                {
                    AreEqual(data[i], data[j]);
                }
                else
                {
                    AreNotEqual(data[i], data[j]);
                }
            }
        }
    }

    protected virtual IHttpRequest ConfigureBeforeRequest(IHttpRequest request) => request;

    protected virtual Task ConfigureWebHost(IHostBuilder hostBuilder) => Task.CompletedTask;

    protected virtual string[] GetHostArgs() => new[] { "--urls", BaseUrl };

    protected virtual async Task<T[][]> InternalRunAsync<T>(Func<Task<TextHttpOperationResult<T[]>>>[] funcs)
    {
        var tasks = new List<Task<TextHttpOperationResult<T[]>>>();
        foreach (var func in funcs)
        {
            var task = func();
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);

        Assert.IsFalse(tasks.Any(m => m.Result.Data == null));

        return tasks.Select(m => m.Result.Data).ToArray();
    }

    /// <summary>
    /// 并行请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="requestCount">请求总数</param>
    /// <param name="getRequestFunc">获取请求的方法</param>
    /// <param name="assertAction">对每个请求的断言委托</param>
    /// <returns></returns>
    protected virtual async Task ParallelRequestAsync<T>(int requestCount,
                                                         Func<IHttpRequest> getRequestFunc,
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

    #endregion Protected 方法
}
