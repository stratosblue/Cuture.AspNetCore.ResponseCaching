using System;
using System.Net.Http;

using Cuture.Http;

namespace ResponseCaching.Test.Base;

internal class TestCutureHttpMessageInvokerPool : IHttpMessageInvokerPool
{
    #region Private 字段

    private readonly HttpClient _testHttpClient;

    private readonly HttpMessageInvokerOwner _testHttpClientOwner;

    #endregion Private 字段

    #region Public 构造函数

    public TestCutureHttpMessageInvokerPool(HttpClient testHttpClient)
    {
        _testHttpClient = testHttpClient ?? throw new ArgumentNullException(nameof(testHttpClient));
        _testHttpClientOwner = new(_testHttpClient);
    }

    #endregion Public 构造函数

    #region Public 方法

    public void Dispose()
    {
    }

    public IOwner<HttpMessageInvoker> Rent(IHttpRequest request) => _testHttpClientOwner;

    #endregion Public 方法

    #region Private 类

    private record class HttpMessageInvokerOwner(HttpMessageInvoker Value) : IOwner<HttpMessageInvoker>
    {
        #region Public 方法

        public void Dispose() { }

        #endregion Public 方法
    }

    #endregion Private 类
}
