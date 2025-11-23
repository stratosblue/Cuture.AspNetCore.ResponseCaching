using Cuture.AspNetCore.ResponseCaching.Interceptors;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;

namespace ResponseCaching.Test.Interceptors;

[TestClass]
public class InterceptorAggregatorTest
{
    #region Public 方法

    [TestMethod]
    public void ShouldCallImplementedInterfaceMethod()
    {
        var random = new Random();

        for (int i = 0; i < 100; i++)
        {
            var interceptor1 = new Interceptor1(random.Next());
            var interceptor2 = new Interceptor2(random.Next());
            var interceptor3 = new Interceptor3(random.Next());
            var interceptor4 = new Interceptor4(random.Next());

            var interceptors = new IResponseCachingInterceptor[] { interceptor1, interceptor2, interceptor3, interceptor4 }.OrderBy(m => m.Order).ToArray();

            Console.WriteLine($"time: {i:D3} - order:{string.Join(" -> ", interceptors.Select(m => m.GetType().Name))}");

            var interceptorAggregator = new InterceptorAggregator(interceptors);

            interceptorAggregator.OnCacheStoringAsync(null!, null!, null!, static (_, _, _) => Task.FromResult<ResponseCacheEntry?>(null));
            interceptorAggregator.OnResponseWritingAsync(null!, null!, static (_, _) => Task.FromResult(true));

            NormalCheck(interceptors);
        }
    }

    [TestMethod]
    public void ShouldShortCircuits()
    {
        var random = new Random();

        for (int i = 0; i < 100; i++)
        {
            var interceptor1 = new Interceptor1(random.Next());
            var interceptor2 = new Interceptor2(random.Next());
            var interceptor3 = new Interceptor3(random.Next());
            var interceptor4 = new Interceptor4(random.Next());
            var shortCircuitsInterceptor = new ShortCircuitsInterceptor(random.Next());

            var interceptors = new IResponseCachingInterceptor[] { interceptor1, interceptor2, interceptor3, interceptor4, shortCircuitsInterceptor }.OrderBy(m => m.Order).ToArray();

            Console.WriteLine($"time: {i:D3} - order:{string.Join(" -> ", interceptors.Select(m => m.GetType().Name))}");

            var interceptorAggregator = new InterceptorAggregator(interceptors);

            interceptorAggregator.OnCacheStoringAsync(null!, null!, null!, static (_, _, _) => Task.FromResult<ResponseCacheEntry?>(null));
            interceptorAggregator.OnResponseWritingAsync(null!, null!, static (_, _) => Task.FromResult(true));

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

                break;

            case Interceptor2 interceptor2:
                Assert.AreEqual(1, interceptor2.OnCacheStoringCallCount);
                Assert.AreEqual(1, interceptor2.OnResponseWritingCallCount);

                break;

            case Interceptor3 interceptor3:

                Assert.AreEqual(1, interceptor3.OnCacheStoringCallCount);
                Assert.AreEqual(1, interceptor3.OnResponseWritingCallCount);

                break;

            case Interceptor4 interceptor4:

                Assert.AreEqual(1, interceptor4.OnCacheStoringCallCount);
                Assert.AreEqual(0, interceptor4.OnResponseWritingCallCount);

                break;

            case ShortCircuitsInterceptor shortCircuitsInterceptor:

                Assert.AreEqual(1, shortCircuitsInterceptor.OnCacheStoringCallCount);
                Assert.AreEqual(1, shortCircuitsInterceptor.OnResponseWritingCallCount);

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
    }

    #endregion Public 方法

    #region Private 类

    private interface IInterceptorCallCountable
    {
        #region Public 属性

        int OnCacheStoringCallCount { get; set; }

        int OnResponseWritingCallCount { get; set; }

        #endregion Public 属性
    }

    private class Interceptor1 : IResponseWritingInterceptor, IInterceptorCallCountable
    {
        #region Public 字段

        public int OnCacheStoringCallCount { get; set; }

        public int OnResponseWritingCallCount { get; set; }

        public int Order { get; }

        #endregion Public 字段

        #region Public 构造函数

        public Interceptor1(int order)
        {
            Order = order;
        }

        #endregion Public 构造函数

        #region Public 方法

        public Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next)
        {
            OnResponseWritingCallCount++;
            return next(actionContext, entry);
        }

        #endregion Public 方法
    }

    private class Interceptor2 : IResponseWritingInterceptor, ICacheStoringInterceptor, IInterceptorCallCountable
    {
        #region Public 字段

        public int OnCacheStoringCallCount { get; set; }

        public int OnResponseWritingCallCount { get; set; }

        public int Order { get; }

        #endregion Public 字段

        #region Public 构造函数

        public Interceptor2(int order)
        {
            Order = order;
        }

        #endregion Public 构造函数

        #region Public 方法

        public Task<ResponseCacheEntry?> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate<ActionContext> next)
        {
            OnCacheStoringCallCount++;
            return next(actionContext, key, entry);
        }

        public Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next)
        {
            OnResponseWritingCallCount++;
            return next(actionContext, entry);
        }

        #endregion Public 方法
    }

    private class Interceptor3 : IResponseWritingInterceptor, ICacheStoringInterceptor, IInterceptorCallCountable
    {
        #region Public 字段

        public int OnCacheStoringCallCount { get; set; }

        public int OnResponseWritingCallCount { get; set; }

        public int Order { get; }

        #endregion Public 字段

        #region Public 构造函数

        public Interceptor3(int order)
        {
            Order = order;
        }

        #endregion Public 构造函数

        #region Public 方法

        public Task<ResponseCacheEntry?> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate<ActionContext> next)
        {
            OnCacheStoringCallCount++;
            return next(actionContext, key, entry);
        }

        public Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next)
        {
            OnResponseWritingCallCount++;
            return next(actionContext, entry);
        }

        #endregion Public 方法
    }

    private class Interceptor4 : ICacheStoringInterceptor, IInterceptorCallCountable
    {
        #region Public 字段

        public int OnCacheStoringCallCount { get; set; }

        public int OnResponseWritingCallCount { get; set; }

        public int Order { get; }

        #endregion Public 字段

        #region Public 构造函数

        public Interceptor4(int order)
        {
            Order = order;
        }

        #endregion Public 构造函数

        #region Public 方法

        public Task<ResponseCacheEntry?> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate<ActionContext> next)
        {
            OnCacheStoringCallCount++;
            return next(actionContext, key, entry);
        }

        #endregion Public 方法
    }

    private class ShortCircuitsInterceptor : IResponseWritingInterceptor, ICacheStoringInterceptor, IInterceptorCallCountable
    {
        #region Public 字段

        public int OnCacheStoringCallCount { get; set; }

        public int OnResponseWritingCallCount { get; set; }

        public int Order { get; }

        #endregion Public 字段

        #region Public 构造函数

        public ShortCircuitsInterceptor(int order)
        {
            Order = order;
        }

        #endregion Public 构造函数

        #region Public 方法

        public Task<ResponseCacheEntry?> OnCacheStoringAsync(ActionContext actionContext, string key, ResponseCacheEntry entry, OnCacheStoringDelegate<ActionContext> next)
        {
            OnCacheStoringCallCount++;
            return Task.FromResult<ResponseCacheEntry?>(null);
        }

        public Task<bool> OnResponseWritingAsync(ActionContext actionContext, ResponseCacheEntry entry, OnResponseWritingDelegate next)
        {
            OnResponseWritingCallCount++;
            return Task.FromResult(true);
        }

        #endregion Public 方法
    }

    #endregion Private 类
}
