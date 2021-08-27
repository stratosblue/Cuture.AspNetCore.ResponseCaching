using System;
using System.Collections.Generic;

using Cuture.AspNetCore.ResponseCaching.Internal;
using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseCaching.Lockers
{
    internal class DefaultActionExecutingLockerCreator : ExecutingLockerCreator
    {
        #region Public 构造函数

        public DefaultActionExecutingLockerCreator(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        #endregion Public 构造函数

        #region Protected 方法

        protected override object CreateLocker()
        {
            return new DefaultActionExecutingLocker(RequiredOptions<ResponseCachingOptions>(), CreateExecutionLockStatePool<IActionResult>());
        }

        #endregion Protected 方法
    }

    internal class DefaultActionSingleActionExecutingLockerCreator : ExecutingLockerCreator
    {
        #region Private 字段

        private readonly ExecutionLockStatePool<IActionResult> _executionLockStatePool;

        #endregion Private 字段

        #region Public 构造函数

        public DefaultActionSingleActionExecutingLockerCreator(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _executionLockStatePool = CreateExecutionLockStatePool<IActionResult>();
        }

        #endregion Public 构造函数

        #region Protected 方法

        protected override object CreateLocker()
        {
            return new DefaultActionSingleActionExecutingLocker(RequiredOptions<ResponseCachingOptions>(), _executionLockStatePool);
        }

        #endregion Protected 方法
    }

    internal class DefaultActionSingleResourceExecutingLockerCreator : ExecutingLockerCreator
    {
        #region Private 字段

        private readonly ExecutionLockStatePool<ResponseCacheEntry> _executionLockStatePool;

        #endregion Private 字段

        #region Public 构造函数

        public DefaultActionSingleResourceExecutingLockerCreator(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _executionLockStatePool = CreateExecutionLockStatePool<ResponseCacheEntry>();
        }

        #endregion Public 构造函数

        #region Protected 方法

        protected override object CreateLocker()
        {
            return new DefaultActionSingleResourceExecutingLocker(RequiredOptions<ResponseCachingOptions>(), _executionLockStatePool);
        }

        #endregion Protected 方法
    }

    internal class DefaultResourceExecutingLockerCreator : ExecutingLockerCreator
    {
        #region Public 构造函数

        public DefaultResourceExecutingLockerCreator(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        #endregion Public 构造函数

        #region Protected 方法

        protected override object CreateLocker()
        {
            return new DefaultResourceExecutingLocker(RequiredOptions<ResponseCachingOptions>(), CreateExecutionLockStatePool<ResponseCacheEntry>());
        }

        #endregion Protected 方法
    }

    internal abstract class ExecutingLockerCreator
    {
        #region Private 字段

        private readonly Dictionary<string, object> _lockers = new();

        #endregion Private 字段

        #region Protected 属性

        protected IServiceProvider ServiceProvider { get; }

        #endregion Protected 属性

        #region Public 构造函数

        public ExecutingLockerCreator(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        #endregion Public 构造函数

        #region Public 方法

        public object CreateLocker(string name)
        {
            object? cachedLocker;
            lock (_lockers)
            {
                if (!_lockers.TryGetValue(name, out cachedLocker))
                {
                    cachedLocker = CreateLocker();
                    _lockers.Add(name, cachedLocker);
                }
            }
            return cachedLocker;
        }

        #endregion Public 方法

        #region Protected 方法

        protected ExecutionLockStatePool<TStatePayload> CreateExecutionLockStatePool<TStatePayload>() where TStatePayload : class
        {
            return new ExecutionLockStatePool<TStatePayload>(RequiredService<INakedBoundedObjectPool<ExecutionLockState<TStatePayload>>>());
        }

        protected abstract object CreateLocker();

        protected IOptions<TOptions> RequiredOptions<TOptions>() where TOptions : class, new()
            => ServiceProvider.GetRequiredService<IOptions<TOptions>>();

        protected TService RequiredService<TService>() where TService : notnull
            => ServiceProvider.GetRequiredService<TService>();

        #endregion Protected 方法
    }
}