using System;
using System.Linq;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseCaching.Interceptors;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ResponseCaching.Test.Interceptors
{
    [TestClass]
    public class CachingProcessInterceptorSkipCallTest
    {
        #region Public 方法

        [TestMethod]
        public void ShouldNotCallSkipCallMethod()
        {
            var random = new Random();

            for (int i = 0; i < 100; i++)
            {
                var interceptor1 = new Interceptor1();
                var interceptor2 = new Interceptor2();
                var interceptor3 = new Interceptor3();
                var interceptor4 = new Interceptor4();

                var interceptors = new ICachingProcessInterceptor[] { interceptor1, interceptor2, interceptor3, interceptor4 };

                //随机组合进行测试
                interceptors = interceptors.OrderBy(_ => random.Next()).ToArray();

                Console.WriteLine($"time: {i:D3} - order:{string.Join(" -> ", interceptors.Select(m => m.GetType().Name))}");

                var interceptorAggregator = new InterceptorAggregator(interceptors);

                interceptorAggregator.OnCacheStoringAsync(null, null, null, (_, _, _) => Task.FromResult<ResponseCacheEntry>(null));
                interceptorAggregator.OnResponseWritingAsync(null, null, (_, _) => Task.FromResult(true));
                interceptorAggregator.OnResultSettingAsync(null, null, (_, _) => Task.CompletedTask);

                Assert.AreEqual(0, interceptor1.OnCacheStoringCallCount);
                Assert.AreEqual(1, interceptor1.OnResponseWritingCallCount);
                Assert.AreEqual(0, interceptor1.OnResultSettingCallCount);

                Assert.AreEqual(1, interceptor2.OnCacheStoringCallCount);
                Assert.AreEqual(1, interceptor2.OnResponseWritingCallCount);
                Assert.AreEqual(1, interceptor2.OnResultSettingCallCount);

                Assert.AreEqual(0, interceptor3.OnCacheStoringCallCount);
                Assert.AreEqual(0, interceptor3.OnResponseWritingCallCount);
                Assert.AreEqual(0, interceptor3.OnResultSettingCallCount);

                Assert.AreEqual(1, interceptor4.OnCacheStoringCallCount);
                Assert.AreEqual(0, interceptor4.OnResponseWritingCallCount);
                Assert.AreEqual(0, interceptor4.OnResultSettingCallCount);
            }
        }

        #endregion Public 方法

        #region Private 类

        private class Interceptor1 : CachingProcessInterceptor
        {
            #region Public 字段

            public int OnCacheStoringCallCount = 0;
            public int OnResponseWritingCallCount = 0;
            public int OnResultSettingCallCount = 0;

            #endregion Public 字段

            #region Public 方法

            [SkipCall]
            public override Task<ResponseCacheEntry> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate next)
            {
                OnCacheStoringCallCount++;
                throw new NotImplementedException();
            }

            public override Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next)
            {
                OnResponseWritingCallCount++;
                return base.OnResponseWritingAsync(actionContext, entry, next);
            }

            [SkipCall]
            public override Task OnResultSettingAsync(ActionExecutingContext actionContext, IActionResult actionResult, OnResultSettingDelegate next)
            {
                OnResultSettingCallCount++;
                throw new NotImplementedException();
            }

            #endregion Public 方法
        }

        private class Interceptor2 : CachingProcessInterceptor
        {
            #region Public 字段

            public int OnCacheStoringCallCount = 0;
            public int OnResponseWritingCallCount = 0;
            public int OnResultSettingCallCount = 0;

            #endregion Public 字段

            #region Public 方法

            public override Task<ResponseCacheEntry> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate next)
            {
                OnCacheStoringCallCount++;
                return base.OnCacheStoringAsync(actionContext, key, entry, next);
            }

            public override Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next)
            {
                OnResponseWritingCallCount++;
                return base.OnResponseWritingAsync(actionContext, entry, next);
            }

            public override Task OnResultSettingAsync(ActionExecutingContext actionContext, IActionResult actionResult, OnResultSettingDelegate next)
            {
                OnResultSettingCallCount++;
                return base.OnResultSettingAsync(actionContext, actionResult, next);
            }

            #endregion Public 方法
        }

        private class Interceptor3 : CachingProcessInterceptor
        {
            #region Public 字段

            public int OnCacheStoringCallCount = 0;
            public int OnResponseWritingCallCount = 0;
            public int OnResultSettingCallCount = 0;

            #endregion Public 字段

            #region Public 方法

            [SkipCall]
            public override Task<ResponseCacheEntry> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate next)
            {
                OnCacheStoringCallCount++;
                throw new NotImplementedException();
            }

            [SkipCall]
            public override Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next)
            {
                OnResponseWritingCallCount++;
                throw new NotImplementedException();
            }

            [SkipCall]
            public override Task OnResultSettingAsync(ActionExecutingContext actionContext, IActionResult actionResult, OnResultSettingDelegate next)
            {
                OnResultSettingCallCount++;
                throw new NotImplementedException();
            }

            #endregion Public 方法
        }

        private class Interceptor4 : CachingProcessInterceptor
        {
            #region Public 字段

            public int OnCacheStoringCallCount = 0;
            public int OnResponseWritingCallCount = 0;
            public int OnResultSettingCallCount = 0;

            #endregion Public 字段

            #region Public 方法

            public override Task<ResponseCacheEntry> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate next)
            {
                OnCacheStoringCallCount++;
                return base.OnCacheStoringAsync(actionContext, key, entry, next);
            }

            [SkipCall]
            public override Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next)
            {
                OnResponseWritingCallCount++;
                throw new NotImplementedException();
            }

            [SkipCall]
            public override Task OnResultSettingAsync(ActionExecutingContext actionContext, IActionResult actionResult, OnResultSettingDelegate next)
            {
                OnResultSettingCallCount++;
                throw new NotImplementedException();
            }

            #endregion Public 方法
        }

        #endregion Private 类
    }
}