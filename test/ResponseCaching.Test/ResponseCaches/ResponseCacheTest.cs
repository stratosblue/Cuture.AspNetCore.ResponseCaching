using System.Text;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ResponseCaching.Test.ResponseCaches;

[TestClass]
public abstract class ResponseCacheTest
{
    #region Public 字段

    public const string SimResponseContent = "🤣😂😊❤😍😒👌😘💕😁👍🙌🤦‍♀️🤦‍♂️🤷‍♀️🤷‍♂️✌🤞😉😎🎶😢💖😜👏💋🌹🎉🎂🤳🐱‍👤🐱‍🏍🐱‍💻🐱‍🐉🐱‍👓🐱‍🚀✔👀😃✨😆🤔🤢🎁经验+3，告辞經驗+3，告辭Exp + 3, goodbyeتجربة تركопыт + 3Expérience + 3, au revoir.एक्सप्ट + 3, बाई経験＋3、失礼しますແກ້ນພິມ +3, ລາກ່ອນExp + 3, до побаченняออกเดินทางจากประสบการณ์";

    #endregion Public 字段

    #region Protected 属性

    protected IResponseCache ResponseCache { get; set; }

    #endregion Protected 属性

    #region Public 方法

    [TestCleanup]
    public virtual void Cleanup()
    {
        ResponseCache = null;
    }

    [TestMethod]
    public async Task GetSetResponseEntry()
    {
        //注意：redis所在系统时间与运行测试所在系统时间有误差时，会导致测试无法通过
        var duration = 2;
        var key = "ResponseCacheTestKey";
        var contentType = "application/json; charset=utf-8";
        var body = Encoding.UTF8.GetBytes(SimResponseContent);
        var entry = new ResponseCacheEntry(contentType, body, duration);

        await ResponseCache.SetAsync(key, entry, CancellationToken.None);

        var cachedEntry = await ResponseCache.GetAsync(key, CancellationToken.None);

        Assert.IsNotNull(cachedEntry, "获取到缓存为空，redis所在系统时间与运行测试所在系统时间有误差时，可能会导致测试无法通过");

        TestUtil.EntryEquals(entry, cachedEntry);

        await Task.Delay(TimeSpan.FromSeconds(duration + 2));

        cachedEntry = await ResponseCache.GetAsync(key, CancellationToken.None);

        Assert.IsNull(cachedEntry);
    }

    [TestInitialize]
    public virtual async Task InitAsync()
    {
        ResponseCache = await GetResponseCache();
    }

    [TestMethod]
    public async Task ParallelGetSetResponseEntry()
    {
        //注意：redis所在系统时间与运行测试所在系统时间有误差时，会导致测试无法通过
        var duration = 1;

        var tasks = Enumerable.Range(0, 500).Select(async index =>
        {
            var key = $"ResponseCacheTestKey_{index}";
            var contentType = $"application/json; charset=utf-8; idx={index}";
            var body = Encoding.UTF8.GetBytes($"{SimResponseContent}_{index}");
            var entry = new ResponseCacheEntry(contentType, body, duration);

            await ResponseCache.SetAsync(key, entry, CancellationToken.None);

            var cachedEntry = await ResponseCache.GetAsync(key, CancellationToken.None);

            TestUtil.EntryEquals(entry, cachedEntry);

            await Task.Delay(TimeSpan.FromSeconds(duration + 1));

            cachedEntry = await ResponseCache.GetAsync(key, CancellationToken.None);

            Assert.IsNull(cachedEntry);
        });

        await Task.WhenAll(tasks);
    }

    #endregion Public 方法

    #region Protected 方法

    protected abstract Task<IResponseCache> GetResponseCache();

    #endregion Protected 方法
}
