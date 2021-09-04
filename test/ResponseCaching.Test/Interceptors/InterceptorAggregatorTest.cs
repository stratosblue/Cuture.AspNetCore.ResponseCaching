﻿using System;
using System.Collections.Generic;
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
    public class InterceptorAggregatorTest
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

                var interceptors = new IResponseCachingInterceptor[] { interceptor1, interceptor2, interceptor3, interceptor4 };

                //随机组合进行测试
                interceptors = interceptors.OrderBy(_ => random.Next()).ToArray();

                Console.WriteLine($"time: {i:D3} - order:{string.Join(" -> ", interceptors.Select(m => m.GetType().Name))}");

                var interceptorAggregator = new InterceptorAggregator(interceptors);

                interceptorAggregator.OnCacheStoringAsync(null, null, null, (_, _, _) => Task.FromResult<ResponseCacheEntry>(null));
                interceptorAggregator.OnResponseWritingAsync(null, null, (_, _) => Task.FromResult(true));
                interceptorAggregator.OnResultSettingAsync(null, null, (_, _) => Task.CompletedTask);

                NormalCheck(interceptors);
            }
        }

        [TestMethod]
        public void ShouldShortCircuits()
        {
            var random = new Random();

            for (int i = 0; i < 100; i++)
            {
                var interceptor1 = new Interceptor1();
                var interceptor2 = new Interceptor2();
                var interceptor3 = new Interceptor3();
                var interceptor4 = new Interceptor4();
                var shortCircuitsInterceptor = new ShortCircuitsInterceptor();

                var interceptors = new IResponseCachingInterceptor[] { interceptor1, interceptor2, interceptor3, interceptor4, shortCircuitsInterceptor };

                //随机组合进行测试
                interceptors = interceptors.OrderBy(_ => random.Next()).ToArray();

                Console.WriteLine($"time: {i:D3} - order:{string.Join(" -> ", interceptors.Select(m => m.GetType().Name))}");

                var interceptorAggregator = new InterceptorAggregator(interceptors);

                interceptorAggregator.OnCacheStoringAsync(null, null, null, (_, _, _) => Task.FromResult<ResponseCacheEntry>(null));
                interceptorAggregator.OnResponseWritingAsync(null, null, (_, _) => Task.FromResult(true));
                interceptorAggregator.OnResultSettingAsync(null, null, (_, _) => Task.CompletedTask);

                bool shortCircuited = false;
                foreach (var interceptor in interceptors)
                {
                    if (shortCircuited)
                    {
                        ShouldNoAnyCalled(interceptor);
                    }
                    else
                    {
                        NormalCheck(interceptor);
                        shortCircuited = interceptor is ShortCircuitsInterceptor;
                    }
                }
            }
        }

        private static void NormalCheck(IEnumerable<IResponseCachingInterceptor> interceptors)
        {
            foreach (var interceptor in interceptors)
            {
                NormalCheck(interceptor);
            }
        }

        private static void NormalCheck(IResponseCachingInterceptor interceptor)
        {
            switch (interceptor)
            {
                case Interceptor1 interceptor1:
                    Assert.AreEqual(0, interceptor1.OnCacheStoringCallCount);
                    Assert.AreEqual(1, interceptor1.OnResponseWritingCallCount);
                    Assert.AreEqual(0, interceptor1.OnResultSettingCallCount);

                    break;

                case Interceptor2 interceptor2:
                    Assert.AreEqual(1, interceptor2.OnCacheStoringCallCount);
                    Assert.AreEqual(1, interceptor2.OnResponseWritingCallCount);
                    Assert.AreEqual(1, interceptor2.OnResultSettingCallCount);
                    break;

                case Interceptor3 interceptor3:

                    Assert.AreEqual(1, interceptor3.OnCacheStoringCallCount);
                    Assert.AreEqual(1, interceptor3.OnResponseWritingCallCount);
                    Assert.AreEqual(1, interceptor3.OnResultSettingCallCount);

                    break;

                case Interceptor4 interceptor4:

                    Assert.AreEqual(1, interceptor4.OnCacheStoringCallCount);
                    Assert.AreEqual(0, interceptor4.OnResponseWritingCallCount);
                    Assert.AreEqual(0, interceptor4.OnResultSettingCallCount);
                    break;

                case ShortCircuitsInterceptor shortCircuitsInterceptor:

                    Assert.AreEqual(1, shortCircuitsInterceptor.OnCacheStoringCallCount);
                    Assert.AreEqual(1, shortCircuitsInterceptor.OnResponseWritingCallCount);
                    Assert.AreEqual(1, shortCircuitsInterceptor.OnResultSettingCallCount);
                    break;

                default:
                    break;
            }
        }

        private static void ShouldNoAnyCalled(IResponseCachingInterceptor interceptor)
        {
            var callCountable = interceptor as IInterceptorCallCountable;

            Assert.IsNotNull(callCountable);

            Assert.AreEqual(0, callCountable.OnCacheStoringCallCount, interceptor.GetType().Name);
            Assert.AreEqual(0, callCountable.OnResponseWritingCallCount, interceptor.GetType().Name);
            Assert.AreEqual(0, callCountable.OnResultSettingCallCount, interceptor.GetType().Name);
        }

        #endregion Public 方法

        #region Private 类

        private interface IInterceptorCallCountable
        {
            #region Public 属性

            int OnCacheStoringCallCount { get; set; }
            int OnResponseWritingCallCount { get; set; }
            int OnResultSettingCallCount { get; set; }

            #endregion Public 属性
        }

        private class Interceptor1 : IResponseWritingInterceptor, IInterceptorCallCountable
        {
            #region Public 字段

            public int OnCacheStoringCallCount { get; set; }
            public int OnResponseWritingCallCount { get; set; }
            public int OnResultSettingCallCount { get; set; }

            #endregion Public 字段

            #region Public 方法

            public Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next)
            {
                OnResponseWritingCallCount++;
                return next(actionContext, entry);
            }

            #endregion Public 方法
        }

        private class Interceptor2 : IActionResultSettingInterceptor, IResponseWritingInterceptor, ICacheStoringInterceptor, IInterceptorCallCountable
        {
            #region Public 字段

            public int OnCacheStoringCallCount { get; set; }
            public int OnResponseWritingCallCount { get; set; }
            public int OnResultSettingCallCount { get; set; }

            #endregion Public 字段

            #region Public 方法

            public Task<ResponseCacheEntry> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate next)
            {
                OnCacheStoringCallCount++;
                return next(actionContext, key, entry);
            }

            public Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next)
            {
                OnResponseWritingCallCount++;
                return next(actionContext, entry);
            }

            public Task OnResultSettingAsync(ActionExecutingContext actionContext, IActionResult actionResult, OnResultSettingDelegate next)
            {
                OnResultSettingCallCount++;
                return next(actionContext, actionResult);
            }

            #endregion Public 方法
        }

        private class Interceptor3 : IActionResultSettingInterceptor, IResponseWritingInterceptor, ICacheStoringInterceptor, IInterceptorCallCountable
        {
            #region Public 字段

            public int OnCacheStoringCallCount { get; set; }
            public int OnResponseWritingCallCount { get; set; }
            public int OnResultSettingCallCount { get; set; }

            #endregion Public 字段

            #region Public 方法

            public Task<ResponseCacheEntry> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate next)
            {
                OnCacheStoringCallCount++;
                return next(actionContext, key, entry);
            }

            public Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next)
            {
                OnResponseWritingCallCount++;
                return next(actionContext, entry);
            }

            public Task OnResultSettingAsync(ActionExecutingContext actionContext, IActionResult actionResult, OnResultSettingDelegate next)
            {
                OnResultSettingCallCount++;
                return next(actionContext, actionResult);
            }

            #endregion Public 方法
        }

        private class Interceptor4 : ICacheStoringInterceptor, IInterceptorCallCountable
        {
            #region Public 字段

            public int OnCacheStoringCallCount { get; set; }
            public int OnResponseWritingCallCount { get; set; }
            public int OnResultSettingCallCount { get; set; }

            #endregion Public 字段

            #region Public 方法

            public Task<ResponseCacheEntry> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate next)
            {
                OnCacheStoringCallCount++;
                return next(actionContext, key, entry);
            }

            #endregion Public 方法
        }

        private class ShortCircuitsInterceptor : IActionResultSettingInterceptor, IResponseWritingInterceptor, ICacheStoringInterceptor, IInterceptorCallCountable
        {
            #region Public 字段

            public int OnCacheStoringCallCount { get; set; }
            public int OnResponseWritingCallCount { get; set; }
            public int OnResultSettingCallCount { get; set; }

            #endregion Public 字段

            #region Public 方法

            public Task<ResponseCacheEntry> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate next)
            {
                OnCacheStoringCallCount++;
                return Task.FromResult<ResponseCacheEntry>(null);
            }

            public Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next)
            {
                OnResponseWritingCallCount++;
                return Task.FromResult(true);
            }

            public Task OnResultSettingAsync(ActionExecutingContext actionContext, IActionResult actionResult, OnResultSettingDelegate next)
            {
                OnResultSettingCallCount++;
                return Task.CompletedTask;
            }

            #endregion Public 方法
        }

        #endregion Private 类
    }
}