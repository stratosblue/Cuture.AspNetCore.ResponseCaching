﻿using System;
using System.Net.Http;

using Cuture.Http;

namespace ResponseCaching.Test.Base
{
    internal class TestCutureHttpClientFactory : IHttpTurboClientFactory
    {
        #region Private 字段

        private readonly HttpClient _testHttpClient;

        #endregion Private 字段

        #region Public 构造函数

        public TestCutureHttpClientFactory(HttpClient testHttpClient)
        {
            _testHttpClient = testHttpClient ?? throw new ArgumentNullException(nameof(testHttpClient));
        }

        #endregion Public 构造函数

        #region Public 方法

        public void Clear()
        {
        }

        public void Dispose()
        {
        }

        public IHttpTurboClient GetTurboClient(IHttpTurboRequest request)
        {
            return new HttpTurboClient(_testHttpClient, false);
        }

        #endregion Public 方法
    }
}